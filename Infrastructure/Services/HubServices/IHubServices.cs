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
        public Task AddGroupAsync(string GroupName, string connectionId);
        public Task SendMessage(string GroupId, Message message);
    }
}
