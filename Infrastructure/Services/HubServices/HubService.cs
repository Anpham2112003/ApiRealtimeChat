using Domain.Entites;
using Microsoft.AspNetCore.SignalR;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.HubServices
{
    public class HubService : Hub
    {
        
        public async Task AddGroupAsync(string GroupName, string connectionId)
        {
            await Groups.AddToGroupAsync(GroupName, connectionId);
        }

        
        public override Task OnConnectedAsync()
        {
            
            return base.OnConnectedAsync();
        }

        

        public async Task SendMessageToGroup(string GroupName, Message message)
        {
           await Clients.Group(GroupName).SendAsync(GroupName, message);
        }

    }
}
