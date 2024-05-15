using Domain.Entites;
using Infrastructure.MongoDBContext;
using Infrastructure.Repository.BaseRepository;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class MessageFriendRepository : AbstractRepository<FriendMessageCollection>, IMessageFriendRepository
    {
        public MessageFriendRepository(IMongoDB mongoDB) : base(mongoDB)
        {
            base._collection=mongoDB.GetCollection<FriendMessageCollection>(nameof(FriendMessageCollection));
        }

        public async Task<FriendMessageCollection?> CheckFriendMessageCollection(ObjectId id)
        {
            var filter = Builders<FriendMessageCollection>.Filter.Eq(x=>x.Id , id);

            var project = Builders<FriendMessageCollection>.Projection
                .Exclude(x => x.Messages);

            var result = await _collection.Find(filter).Project(project)
                .As<FriendMessageCollection>()
                .FirstOrDefaultAsync(); 
            
                
            return result;
        }

        public async Task CreatedFriendMessageCollection(FriendMessageCollection collection)
        {
            await _collection!.InsertOneAsync(collection);
                
        }
    }
}
