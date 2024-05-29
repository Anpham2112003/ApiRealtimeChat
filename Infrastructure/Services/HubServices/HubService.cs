using Domain.Entities;
using Domain.Ultils;
using Infrastructure.Unit0fWork;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;
using Org.BouncyCastle.Asn1.Ocsp;
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

        public HubService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        
        public async Task JoinGroup(string GroupId)
        {
            var UserId = Context.User!.GetIdFromClaim();

            var check = await _unitOfWork.conversationRepository.GetInforConversation(UserId,GroupId);
            
            if(check == null) Context.Abort();
         

            await Groups.AddToGroupAsync(Context.ConnectionId,GroupId);

            await Clients.Client(Context.ConnectionId).Notification(UserId,"Joined");
        }


       
        




        
        public override  async Task OnConnectedAsync()
        {
            var UserId = Context.User.FindFirstValue(ClaimTypes.PrimarySid);

            await _unitOfWork.userRepository.ChangeStateUserAsync(UserId,Domain.Enums.UserState.Online);

            await Clients.Client(Context.ConnectionId).Notification(UserId, "Connected success!");

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
