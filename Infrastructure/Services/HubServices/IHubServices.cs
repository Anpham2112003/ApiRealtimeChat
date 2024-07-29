using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.HubServices
{
    public interface IHubServices
    {
        
        public Task ReceiveMessage( string conversationId,object[]? message);

        public Task Notification(object message);

        public Task Connection(string message, int state);

    }
}
