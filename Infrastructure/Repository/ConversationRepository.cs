using Domain.Entities;
using Domain.ResponeModel.BsonConvert;
using Infrastructure.MongoDBContext;
using Infrastructure.Repository.BaseRepository;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    internal class ConversationRepository : AbstractRepository<ConversationCollection>, IConversationRepository
    {
        public ConversationRepository(IMongoDB mongoDB) : base(mongoDB)
        {
            _collection = mongoDB.GetCollection<ConversationCollection>(nameof(ConversationCollection));
        }

        public async Task<List<ConversationCollection>> GetAllConversationAsync(string UserId, int skip,int limit)
        {
            var filter = Builders<ConversationCollection>.Filter.Eq("Owners", UserId);

            var projection = Builders<ConversationCollection>.Projection
                .Include(x => x.Id)
                .Include(x => x.Owners)
                .Include(x => x.IsGroup)
                .Slice(x => x.Messages, -10)
                .Slice(x => x.Group!.Members, -10)
                .Include(x => x.CreatedAt)
                .Include(x => x.UpdatedAt);

            var result= await _collection.Find(filter)
                .Project<ConversationCollection>(projection)
                .Skip(skip).Limit(limit).ToListAsync();

            return result;
        }

        public async Task<ConversationCollection?> GetConversation(string from, string to)
        {
            var buider = Builders<ConversationCollection>.Filter;
            var filter = buider.And
                (
                   
                    Builders<ConversationCollection>.Filter.All("Owners", new[] { from, to }),

                    Builders<ConversationCollection>.Filter.Eq(x => x.IsGroup, false)

                );

            var project = Builders<ConversationCollection>.Projection
                .Include(x => x.Id)
                .Include(x => x.Owners)
                .Slice(x=>x.Messages,-10);
               
              


            var result = await _collection.Find(filter).Project<ConversationCollection>(project).FirstOrDefaultAsync();

            return result;

        }

        public async Task<ConversationCollection?> GetInforConversation(string UserId,string ConversationId)
        {
            var filter= Builders<ConversationCollection>.Filter.Where(x=>x.Id == ConversationId&&x.Owners!.Any(x=>x.Equals(UserId)));
            var projection = Builders<ConversationCollection>.Projection.
                Include(x => x.Id)
                .Include(x => x.IsGroup);

            var result = await _collection.Find(filter).Project<ConversationCollection?>(projection).FirstOrDefaultAsync();

            return result;
        }

        public async Task<DeleteResult> RemoveConversation(string ConversationId)
        {
            var result = await _collection.DeleteOneAsync(x=>x.Id==ConversationId);

            return result;
        }
    }
        


        
       
    
}
