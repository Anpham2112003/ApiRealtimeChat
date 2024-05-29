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
        public Task<UpdateResult> RemoveMessage(string ConversationId, string UserId, string MessageId);
        public  Task<UpdateResult> ChangeContentMessage(string ConversationId, string UserId, string MessageId, string Content);
        public Task<UpdateResult> PindMessage(string ConversationId, PindMessage message);
        public Task<UpdateResult> UnPindMessage(string ConversationId,string UserId, string MessageId);
        public Task<List<Message>> GetMessagesAsync(string ConversationId, int skip, int limit);
    }
}
