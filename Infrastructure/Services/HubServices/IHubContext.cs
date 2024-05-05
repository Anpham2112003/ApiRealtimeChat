using Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.HubServices
{
    public interface IHubContext
    {
        public Task AddGroupAsync(string GroupName, string connectionId);
        public Task SendMessageToGroup(string GroupName, Message message);
    }
}
