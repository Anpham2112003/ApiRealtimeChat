using Domain.Entites;
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
    [Authorize]
    public class HubService : Hub
    {
       private readonly IUnitOfWork _unitOfWork;

        public HubService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        
        public async Task AddGroupAsync(string GroupName, string connectionId)
        {
            await Groups.AddToGroupAsync(GroupName, connectionId);
        }




        
        public override  async Task OnConnectedAsync()
        {
            var AccountId = Context.User.FindFirstValue(ClaimTypes.PrimarySid);

            await  _unitOfWork.userRepository.ChangeStateUserAsync(ObjectId.Parse(AccountId));

            Console.WriteLine("User Connected "+ AccountId);
           await Clients.Client(Context.ConnectionId).SendAsync("dddd");
            await base.OnConnectedAsync();
            
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var AccountId = Context.User.FindFirstValue(ClaimTypes.PrimarySid);

             await _unitOfWork.userRepository.ChangeStateUserAsync(ObjectId.Parse(AccountId));

             await base.OnDisconnectedAsync(exception);
        }



        public async Task SendMessageToGroup(string GroupName, Message message)
        {
           await Clients.Group(GroupName).SendAsync(GroupName, message);
        }

    }
}
