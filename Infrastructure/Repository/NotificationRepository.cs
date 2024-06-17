using Domain.Entities;
using Domain.Enums;
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
    public class NotificationRepository : AbstractRepository<NotificationCollection>, INotificationRepository
    {
        public NotificationRepository(IMongoDB mongoDB) : base(mongoDB)
        {
            base._collection=mongoDB.GetCollection<NotificationCollection>(nameof(NotificationCollection));

            var index = Builders<NotificationCollection>.IndexKeys.Ascending(x => x.AccountId);

            var expireIndex = Builders<NotificationCollection>.IndexKeys.Ascending("Notifications._id");

            var indexs = new[]
            {
                new CreateIndexModel<NotificationCollection>(index,new CreateIndexOptions{Unique=true}),
                new CreateIndexModel<NotificationCollection>(expireIndex,new CreateIndexOptions{ExpireAfter=TimeSpan.FromDays(60)}),
            };

            _collection.Indexes.CreateMany(indexs);
           
            
        }

        public async Task AddNotification(string Id, Notification notification)
        {
            var filter = Builders<NotificationCollection>.Filter.Eq(x=>x.AccountId,Id);
            var update = Builders<NotificationCollection>.Update.Push(x=>x.Notifications,notification);
            await _collection!.UpdateOneAsync(filter,update,new UpdateOptions { IsUpsert=true});
        }


        public async Task<NotificationCollection> GetNotification(string Id , int skip, int limit)
        {
            var  filter = Builders<NotificationCollection>.Filter.Eq(x=>x.AccountId,Id);

            var projection= Builders<NotificationCollection>.Projection
                .Include(x=>x.AccountId)
                .Slice(x=>x.Notifications,skip,limit);

            return await _collection.Find(filter).Project<NotificationCollection>(projection).FirstOrDefaultAsync();
        }

        public async Task<UpdateResult> RemoveNotification(string Id, string NotificationId)
        {
            var filter = Builders<NotificationCollection>.Filter.Eq(x=>x.AccountId,Id);

            var update = Builders<NotificationCollection>.Update.PullFilter(x => x.Notifications, Builders<Notification>.Filter.Eq(x => x.Id, NotificationId));

            return await _collection!.UpdateOneAsync(filter, update);
        }

        public async Task RemoveNotification(string Id, string FromId, NotificationType type)
        {
            var filter = Builders<NotificationCollection>.Filter.Eq(x => x.AccountId, Id);

            var update = Builders<NotificationCollection>.Update.PullFilter(x => x.Notifications,

                Builders<Notification>.Filter.And(
                        Builders<Notification>.Filter.Eq(x => x.From, FromId),
                        Builders<Notification>.Filter.Eq(x => x.Type, type)
                )
            );

            await _collection!.UpdateOneAsync(filter, update);
        }
    }
}
