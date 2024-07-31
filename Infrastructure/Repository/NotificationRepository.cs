using Domain.Entities;
using Domain.Enums;
using Domain.ResponeModel;
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


        public async Task<IEnumerable<NotificationResponseModel>> GetNotification(string Id , int skip, int limit)
        {
            var aggry =await _collection.Aggregate()
                .Match(x => x.AccountId == Id)
                .Project(new BsonDocument
                {
                    {
                        "_id",0
                    },
                    {
                        "Notifications",new BsonDocument
                        {
                            {
                                "$slice",new BsonArray
                                {
                                    "$Notifications",skip,limit
                                }
                            }
                        }
                    }
                })
                .Unwind("Notifications")
                .ReplaceRoot<BsonDocument>("$Notifications")
                .AppendStage<BsonDocument>(new BsonDocument
                {
                    {
                        "$lookup",new BsonDocument
                        {
                            {
                                "from",nameof(UserCollection)
                            },
                            {
                                "localField","From"
                            },
                            {
                                "foreignField","AccountId"
                            },
                            {
                                "pipeline",new BsonArray
                                {
                                    new BsonDocument
                                    {
                                        {
                                            "$project",new BsonDocument
                                            {
                                                {
                                                    "_id",0
                                                },
                                                {
                                                    "AccountId",1
                                                },
                                                {
                                                    "FullName",1
                                                },
                                                {
                                                    "Avatar",1
                                                },
                                                {
                                                    "State",1
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            {
                                "as","Users"
                            }
                        }
                    }
                }).ReplaceRoot<BsonDocument>(new BsonDocument
                {
                    {
                        "$mergeObjects",new BsonArray
                        {
                            "$$ROOT",

                            new BsonDocument
                            {
                                {
                                    "$arrayElemAt",new BsonArray
                                    {
                                        "$Users",0
                                    }
                                }
                            },

                        }
                    }
                })
                .Project(new BsonDocument
                {
                    {
                        "From",0
                    },
                    {
                        "Users",0
                    }
                })
                .As<NotificationResponseModel>().ToListAsync();

            return aggry;
                
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
