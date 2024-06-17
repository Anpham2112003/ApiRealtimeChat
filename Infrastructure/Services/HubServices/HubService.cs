using Domain.Entities;
using Domain.Ultils;
using Infrastructure.Services.RedisSevices;
using Infrastructure.Unit0fWork;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;
using Org.BouncyCastle.Asn1.Ocsp;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.HubServices
{

    
    public class HubService : Hub<IHubServices>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisService _redisService;

        public HubService(IUnitOfWork unitOfWork, IRedisService redisService)
        {
            _unitOfWork = unitOfWork;
            _redisService = redisService;
        }


        private async Task JoinGroups(IEnumerable<string> ids)
        {
            try
            {
                if (ids.Any())
                {
                    foreach (var item in ids)
                    {

                        await Groups.AddToGroupAsync(Context.ConnectionId, item);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


       public async Task JoinGroup(string id)
       {
            try
            {
                var UserId = Context.User!.GetIdFromClaim();

                var check = await _unitOfWork.conversationRepository.GetInforConversation(UserId, id);

                if (check is null) Context.Abort();
            }
            catch (Exception)
            {

                throw;
            }
       }
        

        private async Task JoinNotification(string id)
        {
            try
            {
                await Groups.AddToGroupAsync( Context.ConnectionId,id);
            }
            catch (Exception)
            {

                throw;
            }
        }


        
        public override  async Task OnConnectedAsync()
        {
            var UserId = Context.User.FindFirstValue(ClaimTypes.PrimarySid);

            await _unitOfWork.userRepository.ChangeStateUserAsync(UserId,Domain.Enums.UserState.Online);

            await _redisService.SetHashValueToRedis(UserId, new HashEntry[] { new HashEntry("State", "1") });

            var ids = await _unitOfWork.conversationRepository.GetConversationId(UserId);

            await JoinGroups(ids!);

            await JoinNotification(UserId);
            
            await Clients.Client(Context.ConnectionId).Notification( "Connected success!");

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var UserId = Context.User.FindFirstValue(ClaimTypes.PrimarySid);

            await _unitOfWork.userRepository.ChangeStateUserAsync(UserId,state:Domain.Enums.UserState.Offline);

            await base.OnDisconnectedAsync(exception);
        }



     

    }
}
