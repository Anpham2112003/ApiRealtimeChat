using Domain.Entities;
using Domain.ResponeModel.BsonConvert;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.BaseRepository
{
    public interface IConversationRepository:BaseRepository<ConversationCollection>
    {

        public Task<ConversationConvert?> GetConversation(string from, string to);

        public Task<List<ConversationConvert>> GetAllConversationAsync(string UserId, int skip, int limit);
        public Task<ConversationCollection?> GetInforConversation(string UserId, string ConversationId);
        public  Task<DeleteResult> RemoveConversation(string ConversationId);

    }
}
