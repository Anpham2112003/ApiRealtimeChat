using Domain.Entities;
using Domain.Enums;
using Domain.ResponeModel.BsonConvert;
using Infrastructure.MongoDBContext;
using Infrastructure.Repository.BaseRepository;
using Microsoft.VisualBasic;
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
    public class MessageRepository : AbstractRepository<ConversationCollection>, IMessageRepository
    {
        public MessageRepository(IMongoDB mongoDB) : base(mongoDB)
        {
            _collection = mongoDB.GetCollection<ConversationCollection>(nameof(ConversationCollection));
            
        }

        public async Task<List<Message>> GetMessagesAsync(string ConversationId,int skip,int limit)
        {


            var aggry = await _collection.Aggregate()
                .Match(x => x.Id == ConversationId)
                .AppendStage<BsonDocument>(new BsonDocument
                {
                    {
                        "$addFields", new BsonDocument
                        {
                            {
                                "sliceMessage", new BsonDocument
                                {
                                    {
                                        "$slice", new BsonArray
                                        {
                                            "$Messages",skip, limit
                                        }
                                    }
                                }
                            }
                        }
                    },
                    


                })
                 .AppendStage<BsonDocument>(new BsonDocument
                {
                     {
                        "$lookup", new BsonDocument
                        {
                            {
                                "from",nameof(UserCollection)
                            },
                            {
                                "localField","sliceMessage.AccountId"
                            },
                            {
                                "foreignField","AccountId"
                            },
                            {
                                "pipeline", new BsonArray
                                {
                                    new BsonDocument
                                    {
                                        {
                                            "$project", new BsonDocument
                                            {
                                                {
                                                    "_id",0
                                                },
                                                {
                                                    "AccountId",1
                                                },
                                                {
                                                    "Avatar",1
                                                },
                                                {
                                                    "FistName",1
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
                })




                .Project(new BsonDocument
                {
                    {
                        "_id",0
                    },
                    {
                        "Messages", new BsonDocument
                        {
                            {
                                "$map", new BsonDocument
                                {
                                    {
                                        "input","$sliceMessage"
                                    },
                                    {
                                        "as","item"
                                    },
                                    {
                                        "in", new BsonDocument
                                        {
                                            
                                            {
                                                "$mergeObjects", new BsonArray
                                                {
                                                    "$$item",new BsonDocument
                                                    {
                                                        {
                                                            "User", new BsonDocument
                                                            {
                                                                {
                                                                    "$arrayElemAt", new BsonArray
                                                                    {
                                                                        "$Users",
                                                                        new BsonDocument
                                                                        {
                                                                            {
                                                                                "$indexOfArray", new BsonArray
                                                                                {
                                                                                    "$Users.AccountId",
                                                                                    "$$item.AccountId"
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    },
                                                   
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }).As<GetMessageConvert>().FirstOrDefaultAsync();


           
            return aggry.Messages!;
        }

        public async Task<UpdateResult> SendMessageAsync(string Id,string UserId, Message message)
        {
            var filter = Builders<ConversationCollection>.Filter
               .And
               (
                   Builders<ConversationCollection>.Filter.Eq(x => x.Id, Id),
                   Builders<ConversationCollection>.Filter.Eq("Owners",ObjectId.Parse(UserId))
                );

            var update = Builders<ConversationCollection>.Update
                .Set(x=>x.Seen,DateTime.UtcNow)
                .Push(x => x.Messages, message);

            return await _collection!.UpdateOneAsync(filter, update);
        }

        public async Task<UpdateResult> RemoveMessage(string ConversationId, string UserId,string MessageId)
        {
            var filter = Builders<ConversationCollection>.Filter.And
                (
                    Builders<ConversationCollection>.Filter.Eq(x => x.Id, ConversationId),
                    Builders<ConversationCollection>.Filter.Eq("Owners", UserId),
                    Builders<ConversationCollection>.Filter.ElemMatch(x=>x.Messages,x=>x.Id==MessageId)
                );

            var update = Builders<ConversationCollection>.Update
                .Set(x=>x.Messages.FirstMatchingElement().IsDelete,true)
                .Set(x=>x.Messages.FirstMatchingElement().DeletedAt,DateTime.UtcNow);

            

            var result = await _collection!.UpdateOneAsync(filter, update );
            
            return result;
        }

        public async Task<UpdateResult> ChangeContentMessage(string ConversationId, string UserId, string MessageId,string Content)
        {
            var filter = Builders<ConversationCollection>.Filter.And
                (
                    Builders<ConversationCollection>.Filter.Eq(x => x.Id, ConversationId),

                    Builders<ConversationCollection>.Filter.Eq("Owners", UserId),

                    Builders<ConversationCollection>.Filter
                    .ElemMatch(x=>x.Messages,x=>x.Id==MessageId&&x.AccountId==UserId&&x.MessageType==MessageType.Message)
                 );

            var update = Builders<ConversationCollection> .Update
                .Set(x => x.Messages.FirstMatchingElement().Content, Content)
                .Set(x => x.Messages.FirstMatchingElement().UpdatedAt, DateTime.UtcNow);

            return await _collection!.UpdateOneAsync(filter,update);
        }

        public async Task<UpdateResult> PindMessage(string ConversationId, PindMessage message)
        {
            var filter =Builders<ConversationCollection>.Filter.Eq(x=>x.Id, ConversationId);

            var update = Builders<ConversationCollection>
                .Update.AddToSet(x => x.MessagePinds, message);

            return await _collection!.UpdateOneAsync(filter, update);
        }

        public async Task<UpdateResult> UnPindMessage(string ConversationId,string UserId ,string MessageId)
        {
            var filter = Builders<ConversationCollection>.Filter.Eq(x => x.Id, ConversationId);

            var update = Builders<ConversationCollection>.Update
                .PullFilter(x=>x.MessagePinds,
                Builders<PindMessage>.Filter.And(
                        Builders<PindMessage>.Filter.Eq(x=>x.Id,MessageId),
                        Builders<PindMessage>.Filter.Eq(x=>x.AccountId,UserId)
                        ) 
                );

            return await _collection!.UpdateOneAsync(filter, update);
        }
    }
}
