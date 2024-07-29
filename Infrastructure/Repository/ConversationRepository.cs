using Domain.Entities;
using Domain.ResponeModel;
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
                new CreateIndexModel<ConversationCollection>(ownerIndex),
                new CreateIndexModel<ConversationCollection>(seenIndex),
            };

            _collection.Indexes.CreateMany(indexs);
        }
        
        public async Task<ConversationResponseModel?> GetConversationByIdAsync(string ConversationId, string UserId)
        {
            var filter = Builders<ConversationCollection>.Filter.And(
                Builders<ConversationCollection>.Filter.Eq(x => x.Id, ConversationId),
                Builders<ConversationCollection>.Filter.Eq("Owners", ObjectId.Parse(UserId)),
                Builders<ConversationCollection>.Filter.Eq(x=>x.IsDelete, false),
                Builders<ConversationCollection>.Filter.Ne("Wait",ObjectId.Parse(UserId))
            );


            var aggy = await _collection.Aggregate()
                .Match(filter)
                .AppendStage<BsonDocument>(new BsonDocument
                {
                    {
                        "$addFields", new BsonDocument
                        {
                            {
                                "sliceOwner", new BsonDocument
                                {
                                    {
                                        "$slice", new BsonArray
                                        {
                                            "$Owners",
                                            -2
                                        }
                                    }
                                }
                            },
                            {
                                "sliceMessage", new BsonDocument
                                {
                                    {
                                        "$slice", new BsonArray
                                        {
                                            "$Messages",
                                            -10
                                        }
                                    }
                                }
                            },
                            {
                                "slicePinds",new BsonDocument
                                {
                                    {
                                        "$slice",new BsonArray
                                        {
                                            "$Pinds",-1
                                        }
                                    }
                                }
                            }
                    }   }
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
                                "localField","slicePinds.AccountId"
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
                                                    "State",1
                                                },
                                                {
                                                    "Avatar",1
                                                },
                                              
                                                
                                            }
                                        }
                                    }
                                }
                            },
                            {
                                "as","PindUser"
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
                                                    "FullName",1
                                                },
                                                {
                                                    "State",1
                                                },
                                                {
                                                    "Avatar",1
                                                },
                                                
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
                }).AppendStage<BsonDocument>(new BsonDocument
                {
                    {
                        "$lookup", new BsonDocument
                        {
                            {
                                "from",nameof (UserCollection)
                            },
                            {
                                "localField", "sliceMessage.AccountId"
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
                                                    "State",1
                                                },
                                                {
                                                    "Avatar",1
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
                        "_id",1
                    },
                    {
                        "Owners","$OwnerResult"
                    },
                    {
                        "IsGroup",1
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
                                                    }
                                                    ,
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
                                                                        "Message was delete",
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
                    },
                    {
                        "Group", new BsonDocument
                        {
                            {
                                "Name",1
                            },
                            {
                                "Avatar",1
                            },
                            {
                                "TotalMember",1
                            },
                            {
                                "UpdatedAt",1
                            }
                        }
                    },
                    {
                        "Pinds", new BsonDocument
                        {
                            {
                                "$map", new BsonDocument
                                {
                                    {
                                        "input","$slicePinds"
                                    },
                                    {
                                        "as","item"
                                    },
                                    {
                                        "in", new BsonDocument
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
                                                                        "$PindUser", new BsonDocument
                                                                        {
                                                                            {
                                                                                "$indexOfArray", new BsonArray
                                                                                {
                                                                                    "$PindUser.AccountId","$$item.AccountId"
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
                                                                        "Message was delete",
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
                    },
                    {
                        "Seen",1
                    },
                    {
                        "CreatedAt",1
                    }
                }).As<ConversationResponseModel>().FirstOrDefaultAsync();

            return aggy;
        }

        public async Task<List<ConversationResponseModel>> GetAllConversationAsync(string UserId, int skip,int limit)
        {
            var buider = Builders<ConversationCollection>.Filter;


            var filter = Builders<ConversationCollection>.Filter.And(
                 buider.Eq("Owners", ObjectId.Parse(UserId)),
                 buider.Eq(x=>x.IsDelete,false),
                 buider.Ne("Wait",ObjectId.Parse(UserId))
            );

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
                                "slicePind", new BsonDocument
                                {
                                    {
                                        "$slice",new BsonArray
                                        {
                                            "$Pinds",-1
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
                                                    "FullName",1
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
                                "localField","slicePind.AccountId"
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
                                                    "Sate",1
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            {
                                "as","PindUser"
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
                        "Owners","$OwnerResult"
                    },
                    {
                        "Pinds",new BsonDocument
                        {
                            {
                                "$map", new BsonDocument
                                {
                                    {
                                        "input","$slicePind"
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
                                                    "$$item", new BsonDocument
                                                    {
                                                        {
                                                            "User", new BsonDocument
                                                            {
                                                                {
                                                                    "$arrayElemAt", new BsonArray
                                                                    {
                                                                        "$PindUser", new BsonDocument
                                                                        {
                                                                            {
                                                                                "$indexOfArray", new BsonArray
                                                                                {
                                                                                    "$PindUser.AccountId","$$item.AccountId"
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
                                                                        "Message was delete",
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
                                                                        
                                                                        "$Users",
                                                                        new BsonDocument
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
                                                    }
                                                    ,
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
                                                                        "Message was delete",
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
                    },
                    {
                        "IsGroup",1
                    },
                    {
                        "Group", new BsonDocument
                        {
                            {
                                "Name",1
                            },
                            {
                                "Avatar",1
                            },
                            {
                                "TotalMember",1
                            },
                            {
                                "UpdatedAt",1
                            }
                        }
                    },
                    {
                        "Seen",1
                    },
                    {
                        "CreatedAt",1
                    }
                }).Skip(skip).Limit(limit).Sort(new BsonDocument
                {
                    {
                        "Seen",-1
                    }
                }).As<ConversationResponseModel>().ToListAsync();

            Debug.WriteLine(aggry);

            return aggry;
        }

        public async Task<List<ConversationResponseModel>> GetWaitConversationAsync(string UserId, int skip, int limit)
        {
            var buider = Builders<ConversationCollection>.Filter;


            var filter = Builders<ConversationCollection>.Filter.And(
                   buider.Eq(x => x.IsDelete, false),
                   buider.Eq("Wait", ObjectId.Parse(UserId))
              );

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
                                "slicePind", new BsonDocument
                                {
                                    {
                                        "$slice",new BsonArray
                                        {
                                            "$Pinds",-1
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
                                                    "FullName",1
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
                                "localField","slicePind.AccountId"
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
                                                    "Sate",1
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            {
                                "as","PindUser"
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
                        "Owners","$OwnerResult"
                    },
                    {
                        "Pinds",new BsonDocument
                        {
                            {
                                "$map", new BsonDocument
                                {
                                    {
                                        "input","$slicePind"
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
                                                    "$$item", new BsonDocument
                                                    {
                                                        {
                                                            "User", new BsonDocument
                                                            {
                                                                {
                                                                    "$arrayElemAt", new BsonArray
                                                                    {
                                                                        "$PindUser", new BsonDocument
                                                                        {
                                                                            {
                                                                                "$indexOfArray", new BsonArray
                                                                                {
                                                                                    "$PindUser.AccountId","$$item.AccountId"
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
                                                                        "Message was delete",
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

                                                                        "$Users",
                                                                        new BsonDocument
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
                                                    }
                                                    ,
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
                                                                        "Message was delete",
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
                    },
                    {
                        "IsGroup",1
                    },
                    {
                        "Group", new BsonDocument
                        {
                            {
                                "Name",1
                            },
                            {
                                "Avatar",1
                            },
                            {
                                "TotalMember",1
                            },
                            {
                                "UpdatedAt",1
                            }
                        }
                    },
                    {
                        "Seen",1
                    },
                    {
                        "CreatedAt",1
                    }
                }).Skip(skip).Limit(limit).Sort(new BsonDocument
                {
                    {
                        "Seen",-1
                    }
                }).As<ConversationResponseModel>().ToListAsync();

            Debug.WriteLine(aggry);

            return aggry;
        }

        public async Task<ConversationResponseModel?> GetConversation(string from, string to)
        {
            var buider = Builders<ConversationCollection>.Filter;

            var filter = buider.And
                (
                   
                    Builders<ConversationCollection>.Filter.All("Owners", new [] { ObjectId.Parse(from),ObjectId.Parse(to)}),

                    Builders<ConversationCollection>.Filter.Eq(x => x.IsGroup, false),

                    Builders<ConversationCollection>.Filter.Eq(x => x.IsDelete, false)

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
                                "slicePinds", new BsonDocument
                                {
                                    {
                                        "$slice", new BsonArray
                                        {
                                            "$Pinds",-10
                                    }   }
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
                                                                            "$OwnerResult",
                                                                            new BsonDocument
                                                                            {
                                                                                {
                                                                                    "$indexOfArray", new BsonArray
                                                                                    {
                                                                                         "$OwnerResult.AccountId","$$item.AccountId"
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
                                                                        "Message was delete",
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
                     },
                     {
                         "Pinds",new BsonDocument
                         {
                             {
                                 "$map", new BsonDocument
                                 {
                                     {
                                         "input", "$slicePinds"
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
                                                     "$$item", new BsonDocument
                                                     {
                                                         {
                                                             "User", new BsonDocument
                                                             {
                                                                 {
                                                                     "$arrayElemAt", new BsonArray
                                                                     {
                                                                         "$OwnerResult", new BsonDocument
                                                                         {
                                                                             {
                                                                                 "$indexOfArray", new BsonArray
                                                                                 {
                                                                                     "$OwnerResult.AccountId","$$item.AccountId"
                                                                                 }
                                                                             }
                                                                         }
                                                                     }
                                                                 }
                                                             }
                                                         }
                                                     }, new BsonDocument
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
                                                                        "Message was delete",
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
                     },
                     {
                         "CreatedAt",1
                     }
                 }).As<ConversationResponseModel>().FirstOrDefaultAsync();

            Debug.WriteLine(aggry);
            return aggry;

        }


        public async Task<UpdateResult> RemoveConversation(string ConversationId,string AccountId)
        {
            var filter = Builders<ConversationCollection>.Filter.And(
                Builders<ConversationCollection>.Filter.Eq(x => x.Id, ConversationId),
                Builders<ConversationCollection>.Filter.Eq(x => x.IsGroup, false),
                Builders<ConversationCollection>.Filter.Eq(x => x.IsDelete, false)
            );

            var update = Builders<ConversationCollection>.Update
                .Set(x=>x.DeletedAt,DateTime.UtcNow)
                .Set(x => x.IsDelete,true);

            return await _collection!.UpdateOneAsync(filter,update);
        }


        public async Task<IEnumerable<string?>> GetConversationId(string id)
        {
            var filter = Builders<ConversationCollection>.Filter.Eq("Owners", ObjectId.Parse(id));

            var project = Builders<ConversationCollection>.Projection.Expression(x=>x.Id!.ToString());

            var result = await _collection!.Find(filter).Project<string>(project).ToListAsync();

            return result;
        }

        public async Task<bool> HasInConversation(string conversationId,string MyId)
        {
            var filter = Builders<ConversationCollection>.Filter.And(

                Builders<ConversationCollection>.Filter.Eq(x => x.Id, conversationId),
                Builders<ConversationCollection>.Filter.Eq("Owners", ObjectId.Parse(MyId))
            );

            return await _collection.Find(filter).AnyAsync();
        }
    }
        


        
       
    
}
