using Domain.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.BaseRepository
{
    public interface IMessageRepository:BaseRepository<ConversationCollection>
    {
        public Task<UpdateResult> SendMessageAsync(string Id, string UserId, Message message);
    }
}
