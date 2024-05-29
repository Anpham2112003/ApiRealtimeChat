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

            var ownerIndex = Builders<ConversationCollection>.IndexKeys.Ascending(x => x.Owners);
            var seenIndex= Builders<ConversationCollection>.IndexKeys.Descending(x => x.Seen);

            var indexs = new List<CreateIndexModel<ConversationCollection>>()
            {
                new CreateIndexModel<ConversationCollection>(ownerIndex,new CreateIndexOptions{Unique=true}),
                new CreateIndexModel<ConversationCollection>(seenIndex),
            };

            _collection.Indexes.CreateMany(indexs);
        }

        public async Task<List<ConversationConvert>> GetAllConversationAsync(string UserId, int skip,int limit)
        {
            var filter = Builders<ConversationCollection>.Filter.Eq("Owners", ObjectId.Parse(UserId));

            var aggry = await _collection.Aggregate()
                .Match(filter)
                .AppendStage<BsonDocument>(new BsonDocument
                {
               
                    {
                        "$addFields",new BsonDocument
                        {
                            {
                                "sliceMessage",new BsonDocument
                                {
                                    {
                                        "$slice", new BsonArray
                                        {
                                            "$Messages",-10
                                        }
                                    }
                                }
                            },
                            {
                                "sliceOwner", new BsonDocument
                                {
                                    {
                                        "$slice",new BsonArray
                                        {
                                            "$Owners",-2
                                        }
                                    }
                                }

                            },
                            {
                                "slicePindMessage", new BsonDocument
                                {
                                    {
                                        "$slice", new BsonArray
                                        {
                                            "$MessagePinds",-5
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
                                "localField","sliceOwner"
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
                                                    "FistName",1
                                                },
                                                {
                                                    "Avatar",1
                                                },
                                                {
                                                    "Sate",1
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            {
                                "as","OwnerResult"
                            }
                        }
                    }
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
                        "Owners","$OwnerResult"
                    },
                    {
                        "MessagePinds","$slicePindMessage"
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
                                                    "$$item",
                                                    new BsonDocument
                                                    {
                                                        {
                                                            "User", new BsonDocument
                                                            {
                                                                {
                                                                    "$arrayElemAt", new BsonArray
                                                                    {
                                                                        new BsonDocument
                                                                        {
                                                                            {
                                                                                "$filter", new BsonDocument
                                                                                {
                                                                                    {
                                                                                        "input","$Users"
                                                                                    },
                                                                                    {
                                                                                        "as","user"
                                                                                    },
                                                                                    {
                                                                                        "cond", new BsonDocument
                                                                                        {
                                                                                            {
                                                                                                "$eq", new BsonArray
                                                                                                {
                                                                                                    "$$item.AccountId",
                                                                                                    "$$user.AccountId"
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        },
                                                                        -1
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
                }).As<ConversationConvert>().ToListAsync();

            Debug.WriteLine(aggry);

            return aggry;
        }

        public async Task<ConversationConvert?> GetConversation(string from, string to)
        {
            var buider = Builders<ConversationCollection>.Filter;
            var filter = buider.And
                (
                   
                    Builders<ConversationCollection>.Filter.All("Owners", new List<ObjectId> { ObjectId.Parse(from),ObjectId.Parse(to)}),

                    Builders<ConversationCollection>.Filter.Eq(x => x.IsGroup, false)

                );

            var aggry = await _collection.Aggregate()
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
                                 "localField","Owners"
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
                                                     "FistName",1
                                                 },
                                                 {
                                                     "UserSate",1
                                                 }
                                             }
                                         }
                                     }
                                 }
                             },
                             {
                                 "as","OwnerResult"
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
                                "sliceMessage",new BsonDocument
                                {
                                    {
                                        "$slice", new BsonArray
                                        {
                                            "$Messages",-10
                                        }
                                    }
                                }
                            },
                            {
                                "slicePindMessage", new BsonDocument
                                {
                                    {
                                        "$slice", new BsonArray
                                        {
                                            "$MessagePinds",-5
                                        }
                                    }
                                }
                            }
                            
                        }
                    }
                 })
              
               
                 
                 .Project(new BsonDocument
                 {
                     {
                         "Owners", "$OwnerResult"
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
                                                     "$$item",
                                                     new BsonDocument
                                                     {
                                                         {
                                                             "User", new BsonDocument
                                                               {

                                                                  {
                                                                       "$arrayElemAt", new BsonArray
                                                                       {
                                                                            new BsonDocument
                                                                            {
                                                                                {
                                                                                    "$filter", new BsonDocument
                                                                                    {
                                                                                        {
                                                                                            "input","$OwnerResult"
                                                                                         },
                                                                                        {
                                                                                            "as","user"
                                                                                         },
                                                                                        {
                                                                                            "cond", new BsonDocument
                                                                                            {
                                                                                                {
                                                                                                    "$eq", new BsonArray
                                                                                                    {  
                                                                                                     "$$item.AccountId",
                                                                                                     "$$user.AccountId"
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                         }
                                                                                    }
                                                                                }
                                                                             },
                                                                         -1
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
                 }).As<ConversationConvert>().FirstOrDefaultAsync();

            Debug.WriteLine(aggry);
            return aggry;

        }

        public async Task<ConversationCollection?> GetInforConversation(string UserId,string ConversationId)
        {
            var filter= Builders<ConversationCollection>.Filter.Where(x=>x.Id == ConversationId&&x.Owners!.Any(x=>x.Equals(ObjectId.Parse(UserId))));
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
