using Domain.Entites;
using Domain.Entities;
using Infrastructure.MongoDBContext;
using Infrastructure.Repository.BaseRepository;
using MongoDB.Bson;
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
    public class MessageRepository : AbstractRepository<GroupCollection>, IMessageRepository
    {
        
        public MessageRepository(IMongoDB mongoDB) : base(mongoDB)
        {
            base._collection = mongoDB.GetCollection<GroupCollection>(nameof(GroupCollection));
        }

        public async Task<MesssageCollection?> GetLastMessageCollection(ObjectId GroupId)
        {
            var filter = Builders<GroupCollection>.Filter.Eq(x => x.Id, GroupId);
            var project = Builders<GroupCollection>
                .Projection
                .Expression(x => new
                {
                    Id = x.Messages!.Last().Id,
                    Page = x.Messages!.Last().Page,
                    Count = x.Messages!.Last().Count,
                }); ;



            var result = await _collection.Find(filter).Project(project).FirstOrDefaultAsync();

            if(result is null) return null;

            return new MesssageCollection()
            {
                Id=result.Id,
                Count=result.Count,
                Page=result.Page,
            };
        }
       

        public async Task AddMessageToPage(ObjectId GroupId, int Page ,int Count, Message message)
        {
            var filter = Builders<GroupCollection>.Filter
                .Where(x=>x.Id == GroupId&&x.Messages!.Any(x=>x.Page==Page));

            var update = Builders<GroupCollection>.Update
                .Set(x=>x.Messages.FirstMatchingElement().Count,Count)
                .AddToSet(x => x.Messages.FirstMatchingElement().Messages, message);

            await _collection!.UpdateOneAsync(filter,update);
        }

        public async Task AddMessageCollection(ObjectId GroupId,int Page,Message? message = null)
        {
            var Message = new MesssageCollection
            {
                Id = ObjectId.GenerateNewId(),
                Count = message is null ? 0 : 1,
                Page = Page,
                Messages = new List<Message> { message }
            };

            var filter = Builders<GroupCollection>.Filter.Eq(x => x.Id, GroupId);

            var update = Builders<GroupCollection>.Update.AddToSet(x => x.Messages, Message);

            await _collection!.UpdateOneAsync(filter, update);
        }

        public async Task<List<Message>> GetMessagesAsync(ObjectId GroupId,int Page , int Skip,int Limit)
        {
            var filter = Builders<GroupCollection>.Filter.Where(x=>x.Id== GroupId&&x.Messages!.Any(x=>x.Page==Page));
            var projection = Builders<GroupCollection>.Projection.Expression(x => new
            {
                Messages = x.Messages.FirstMatchingElement().Messages!.Skip(Skip).Take(Limit)
            });

            var result = await _collection.Find(filter).Project(projection).FirstOrDefaultAsync();

            return result.Messages.ToList();
        }
    }
}
