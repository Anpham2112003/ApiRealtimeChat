using Domain.Entites;
using Domain.Entities;
using Domain.ResponeModel.BsonConvert;
using Infrastructure.MongoDBContext;
using Infrastructure.Repository.BaseRepository;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
    public class GroupMessageRepository : AbstractRepository<GroupCollection>, IGroupMessageRepository
    {
        
        public GroupMessageRepository(IMongoDB mongoDB) : base(mongoDB)
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
                }); 



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

        public async Task AddMessageCollection(ObjectId GroupId,int Page,Message message )
        {
            var Message = new MesssageCollection
            {
                Id = ObjectId.GenerateNewId(),
                Count =1,
                Page = Page,
                Messages = new List<Message> { message! }
            };

            var filter = Builders<GroupCollection>.Filter.Eq(x => x.Id, GroupId);

            var update = Builders<GroupCollection>.Update.AddToSet(x => x.Messages, Message);

            await _collection!.UpdateOneAsync(filter, update);
        }

       

        public async Task<ResultQueyMessage> GetMessageAsync(ObjectId GroupId, int page, int skip,int limit)
        {
            var aggry =await _collection.Aggregate()
                .Match(x => x.Id == GroupId)
                
                .AppendStage<BsonDocument>(new BsonDocument
                {
                    {
                        "$addFields",new BsonDocument
                        {
                            {
                                "last", new BsonDocument
                                {
                                    {
                                        "$arrayElemAt",new BsonArray
                                        {
                                            "$Messages",page
                                        }
                                    }
                                }
                            },
                        }
                    },
                })
                .AppendStage<BsonDocument>(new BsonDocument
                {

                    {
                        "$addFields", new BsonDocument
                        {

                            {
                                "MessageJoin", new BsonDocument
                                {
                                    {
                                        "$slice", new BsonArray
                                        {
                                            "$last.Messages",skip,limit
                                        }
                                    }
                                }
                            }
                        }
                    }
                })
                .AppendStage<BsonDocument>(new BsonDocument
                {
                    {
                        "$lookup",new BsonDocument
                        {
                            {
                                "from",nameof(UserCollection)
                            },
                            {
                                "pipeline", new BsonArray
                                {
                                    new BsonDocument
                                    {
                                        {
                                            "$match", new BsonDocument
                                            {
                                                {
                                                    "$expr", new BsonDocument
                                                    {
                                                        {
                                                            "$eq", new BsonArray
                                                            {
                                                                "$MessageJoin.UserId","$UserCollection.AccountId"
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        },
                                        
                                    },
                                    new BsonDocument
                                    {
                                        {
                                            "$project",new BsonDocument
                                            {
                                                {
                                                    "_id",0
                                                },
                                                {
                                                    "FistName",0
                                                },
                                                {
                                                    "LastName",0
                                                },
                                                {
                                                    "UpdatedAt",0
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            {
                                "as","Result"
                            }
                        }
                    }
                })

                .AppendStage<BsonDocument>(new BsonDocument
                {
                    {
                        "$addFields", new BsonDocument
                        {
                            
                            {
                                "Messages", new BsonDocument
                                {
                                    {
                                        "$map",new BsonDocument
                                        {
                                            {
                                                "input", "$MessageJoin"
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
                                                           "$$item",

                                                            new BsonDocument
                                                            {
                                                                {
                                                                    "User",new BsonDocument
                                                                    {
                                                                        {
                                                                            "$arrayElemAt",new BsonArray
                                                                            {
                                                                                "$Result",new BsonDocument
                                                                                {
                                                                                    {
                                                                                        "$indexOfArray", new BsonArray
                                                                                        {
                                                                                            "$Result.AccountId","$$item.UserId"
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
                                    }
                                }
                            }
                        }
                    }
                })
                .AppendStage<BsonDocument>(new BsonDocument
                {
                    {
                        "$unset",new BsonArray
                        {
                            "Messages.UserId",
                            "Messages.UpdatedAt",
                            "Messages.CreatedAt",
                            "Messages.DeletedAt"
                        }
                    },
                })
                .Project(new BsonDocument
                {
                    {
                        "_id",1
                    },
                    
                    {
                        "Messages",1
                    }
                    
                })
                .As<ResultQueyMessage>()
                .FirstOrDefaultAsync();





            return aggry;
            
        }

       

        public  async Task<UpdateResult> RemoveMessageAsync(ObjectId GroupId, int Page, ObjectId MessageId)
        {
            var filter = Builders<GroupCollection>.Filter.Eq(x => x.Id,GroupId);
            var update = Builders<GroupCollection>.Update
                .Set("Messages.$[i].Messages.$[id].DeletedAt", DateTime.UtcNow)
                .Set("Messages.$[i].Messages.$[id].IsDelete", "true");

            var arrayfilter = new[]
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument
                {
                    {
                        "i.Page",Page
                    }
                }),
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument
                {
                    {
                        "id._id",MessageId
                    }
                })
            };

          return await _collection!.UpdateOneAsync(filter, update, new UpdateOptions
            {
                ArrayFilters = arrayfilter
            });
        }

        public async Task UpdateContentMessageAsync(ObjectId GroupId, int Page, ObjectId MessageId, string Content)
        {
            var filter = Builders<GroupCollection>.Filter.Eq(x=>x.Id,GroupId);
            var update = Builders<GroupCollection>.Update
                .Set("Messages.$[i].Messages.$[e].Content", Content)
                .Set("Messages.$[i].Messages.$[e].UpdatedAt", DateTime.UtcNow);

            var arrayFilter = new[]
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument
                {
                    {
                        "i.Page",Page
                    }
                }),

                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument
                {
                    {
                        "e._id",MessageId
                    }
                })
            };

            await _collection!.UpdateOneAsync(filter, update, new UpdateOptions { ArrayFilters = arrayFilter });
        }
    }
}
