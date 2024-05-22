using Domain.Entities;
using Infrastructure.MongoDBContext;
using Infrastructure.Repository.BaseRepository;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class MessageRepository : AbstractRepository<ConversationCollection>, IMessageRepository
    {
        public MessageRepository(IMongoDB mongoDB) : base(mongoDB)
        {
            _collection = mongoDB.GetCollection<ConversationCollection>(nameof(ConversationCollection));
        }

        public async Task<UpdateResult> SendMessageAsync(string Id,string UserId, Message message)
        {
            var filter = Builders<ConversationCollection>.Filter
               .And
               (
                   Builders<ConversationCollection>.Filter.Eq(x => x.Id, Id),
                   Builders<ConversationCollection>.Filter.Eq("Owners",UserId)
                );

            var update = Builders<ConversationCollection>.Update
                .Push(x => x.Messages, message);

            return await _collection!.UpdateOneAsync(filter, update);
        }
    }
}
