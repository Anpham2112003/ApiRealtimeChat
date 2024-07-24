using Domain.Entities;
using Domain.ResponeModel;
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

        public Task<ConversationResponseModel?> GetConversation(string from, string to);

        public Task<List<ConversationResponseModel>> GetAllConversationAsync(string UserId, int skip, int limit);
        public  Task<DeleteResult> RemoveConversation(string ConversationId);
        public  Task<ConversationResponseModel?> GetConversationByIdAsync(string ConversationId, string UserId);
        public  Task<IEnumerable<string?>> GetConversationId(string id);
        public  Task<bool> HasInConversation(string conversationId, string MyId);
    }
}
