using Domain.Entities;
using Domain.Enums;
using Domain.ResponeModel;
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

        public async Task<IEnumerable<ClientMessageResponseModel>> GetMessagesAsync(string ConversationId,int skip,int limit)
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
                                                    "FullName",1
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
                                                    new BsonDocument
                                                    {
                                                        {
                                                            "Content",new BsonDocument
                                                            {
                                                                {
                                                                    "$cond", new BsonArray
                                                                    {
                                                                        new BsonDocument
                                                                        {
                                                                            {
                                                                                "$eq", new BsonArray
                                                                                {
                                                                                    "$$item.IsDelete",true
                                                                                }
                                                                            }
                                                                        },
                                                                        "Message was deleted!",
                                                                        "$$item.Content"
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                })
                .Unwind("Messages")
                .ReplaceRoot<BsonDocument>("$Messages")
                .As<ClientMessageResponseModel>().ToListAsync();


           
            return aggry is null ? Enumerable.Empty<ClientMessageResponseModel>() : aggry;
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
                .Pull(x=>x.Wait,ObjectId.Parse(UserId))
                .Push(x => x.Messages, message);

            return await _collection!.UpdateOneAsync(filter, update);
        }

        public async Task<UpdateResult> SendMessageAsync(string Id, string UserId, IEnumerable<Message> messages)
        {
            var filter = Builders<ConversationCollection>.Filter
               .And
               (
                   Builders<ConversationCollection>.Filter.Eq(x => x.Id, Id),
                   Builders<ConversationCollection>.Filter.Eq("Owners", ObjectId.Parse(UserId))
                );

            var update = Builders<ConversationCollection>.Update
                .Set(x => x.Seen, DateTime.UtcNow)
                .PushEach(x => x.Messages, messages);

            return await _collection!.UpdateOneAsync(filter, update);
        }


        public async Task<Domain.Entities.Message> FindMessage(string Id, string MessageId)
        {
            
            var query = await _collection.Aggregate()
                .Match(x => x.Id == Id)
                .AppendStage<BsonDocument>(new BsonDocument
                {
                    {
                        "$addFields", new BsonDocument
                        {
                            {
                                "findMessage", new BsonDocument
                                {
                                    {
                                        "$filter", new BsonDocument
                                        {
                                            {
                                                "input","$Messages"
                                            },
                                            {
                                                "as","item"
                                            },
                                            {
                                                "cond", new BsonDocument
                                                {
                                                    {
                                                        "$eq", new BsonArray
                                                        {
                                                            "$$item._id",ObjectId.Parse(MessageId)
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }).Unwind("findMessage").ReplaceRoot<BsonDocument>("$findMessage").As<Domain.Entities.Message>().FirstOrDefaultAsync();

                Debug.WriteLine(query);

            return query; 
        }
        public async Task<IEnumerable<string>> RemoveMessage(string ConversationId, string UserId,string MessageId)
        {
            var filter = Builders<ConversationCollection>.Filter.And
                (
                    Builders<ConversationCollection>.Filter.Eq(x => x.Id, ConversationId),
                    Builders<ConversationCollection>.Filter.Eq("Owners", ObjectId.Parse(UserId)),
                    Builders<ConversationCollection>.Filter.ElemMatch(x => x.Messages, x => x.Id == MessageId && x.AccountId == UserId)
                );

            var update = Builders<ConversationCollection>.Update
                .Set(x=>x.Messages.FirstMatchingElement().IsDelete,true)
                .Set(x=>x.Messages.FirstMatchingElement().DeletedAt,DateTime.UtcNow)
                .Set("Pinds.$[m].IsDelete",true)
                .Set("Pinds.$[m].DeletedAt",DateTime.UtcNow);

            var arrayilter = new[]
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument
                {
                    {
                        "m._id",ObjectId.Parse(MessageId)
                    }
                })
            };
            var projection = Builders<ConversationCollection>.Projection.Expression(x => x.Owners!.Select(x=>x.ToString()));

            var result = await _collection!.FindOneAndUpdateAsync(filter, update,new FindOneAndUpdateOptions<ConversationCollection, IEnumerable<string>> {ArrayFilters=arrayilter,Projection=projection} );
            
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

        public async Task<UpdateResult> PindMessage(string ConversationId, Message message)
        {
            var filter = Builders<ConversationCollection>.Filter.Eq(x=>x.Id, ConversationId);

            var update = Builders<ConversationCollection>
                .Update.AddToSet(x => x.Pinds, message);

            return await _collection!.UpdateOneAsync(filter, update);
        }

        public async Task<UpdateResult> UnPindMessage(string ConversationId,string UserId ,string MessageId)
        {
            var filter =  Builders<ConversationCollection>.Filter.Eq(x => x.Id, ConversationId);

            var update = Builders<ConversationCollection>.Update
                .PullFilter(x => x.Pinds, Builders<Message>.Filter.Eq(x => x.Id, MessageId));

            return await _collection!.UpdateOneAsync(filter, update);
        }

        public async Task<IEnumerable<ClientMessageResponseModel>> GetMessagesPind(string conversationid,string userid,int skip,int limit)
        {
            var filter = Builders<ConversationCollection>.Filter.And(
                    Builders<ConversationCollection>.Filter.Eq(x => x.Id, conversationid),
                    Builders<ConversationCollection>.Filter.Eq("Owners", ObjectId.Parse(userid))
                );

            var result = await _collection.Aggregate()
                .Match(filter)
                .AppendStage<BsonDocument>(new BsonDocument
                {
                    {
                        "$lookup", new BsonDocument
                        {
                            {
                                "from",nameof(UserCollection)
                            },
                            {
                                "localField","Pinds.AccountId"
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
                            {"as","Users" }
                        }
                    }
                }).Project(new BsonDocument
                {
                    {
                        "_id",0
                    },
                    {
                        "Pinds", new BsonDocument
                        {
                            {
                                "$map", new BsonDocument
                                {
                                    {
                                        "input","$Pinds"
                                    },
                                    {
                                        "as","item"
                                    },
                                    {
                                        "in",new BsonDocument
                                        {
                                            {
                                                "$mergeObjects",new BsonArray
                                                {
                                                    "$$item", new BsonDocument
                                                    {
                                                        {
                                                            "User", new BsonDocument
                                                            {
                                                                {
                                                                    "$arrayElemAt", new BsonArray
                                                                    {
                                                                        "$Users", new BsonDocument
                                                                        {
                                                                            {
                                                                                "$indexOfArray", new BsonArray
                                                                                {
                                                                                    "$Users.AccountId","$$item.AccountId"
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    },
                                                    new BsonDocument
                                                    {
                                                        {
                                                            "Content",new BsonDocument
                                                            {
                                                                {
                                                                    "$cond", new BsonArray
                                                                    {
                                                                        new BsonDocument
                                                                        {
                                                                            {
                                                                                "$eq",new BsonArray
                                                                                {
                                                                                    "$$item.IsDelete",true
                                                                                }
                                                                            }
                                                                        },
                                                                        "Message was deleted!",
                                                                        "$$item.Content"
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                })
                .Unwind("Pinds")
                .ReplaceRoot<BsonDocument>("$Pinds")
                .As<ClientMessageResponseModel>()
                .ToListAsync();


            return result is null ? Enumerable.Empty<ClientMessageResponseModel>() : result;
        }
    }
}
