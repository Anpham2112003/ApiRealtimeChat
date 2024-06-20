using Amazon.S3.Model;
using Domain.Entities;
using Domain.ResponeModel;
using Infrastructure.MongoDBContext;
using Infrastructure.Repository.BaseRepository;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class FriendRepository : AbstractRepository<FriendCollection>, IFriendRepository
    {
        private IMongoCollection<UserCollection>? _userCollection;
        public FriendRepository(IMongoDB mongoDB) : base(mongoDB)
        {
            base._collection = mongoDB.GetCollection<FriendCollection>(nameof(FriendCollection));
            _userCollection=mongoDB.GetCollection<UserCollection>(nameof(UserCollection));

            var AccountIdIndex = Builders<FriendCollection>.IndexKeys.Ascending(x => x.AccountId);
            
            _collection.Indexes.CreateOne(new CreateIndexModel<FriendCollection>(AccountIdIndex, new CreateIndexOptions { Unique=true}));
        }

        public async Task AddFriendAsync(string AccountId, string FriendId)
        {
            var fillter = Builders<FriendCollection>
                .Filter.And(
                    Builders<FriendCollection>.Filter.Eq(x => x.AccountId, AccountId),
                    Builders<FriendCollection>.Filter.Nin("Friends._id", new[] { ObjectId.Parse(FriendId) })
                );

            var update = Builders<FriendCollection>
                .Update
                .AddToSet(x => x.Friends, new Friend(FriendId));

            await _collection!.UpdateOneAsync(fillter, update, new UpdateOptions() { IsUpsert = true });
        }

        public async Task AcceptFriend(string myId, string WaitListId)
        {
            var filter = Builders<FriendCollection>.Filter.And(
                   Builders<FriendCollection>.Filter.Eq(x => x.AccountId, myId),
                   Builders<FriendCollection>.Filter.Nin("Friends._id", new[] {ObjectId.Parse(WaitListId)})
                   );

            var update = Builders<FriendCollection>

                .Update
                .Pull(x => x.WaitingList, ObjectId.Parse(WaitListId))
                .Push(x => x.Friends, new Friend(WaitListId));

            await _collection!.UpdateOneAsync(filter, update,new UpdateOptions { IsUpsert=true});
        }

        public async Task<UpdateResult> RemoveFriendAsync(string AccountId, string FriendId)
        {
            var filter = Builders<FriendCollection>
                .Filter.Eq(x => x.AccountId, AccountId);
            var pull=Builders<Friend>.Filter.Eq(x=>x.Id, FriendId);
            var update = Builders<FriendCollection>
                .Update
                .PullFilter(x => x.Friends, pull);

            var result = await _collection!.UpdateOneAsync(filter, update);

            return result;
        }

        public async Task<List<UserConvert>> GetFriendAysnc(string AccountId, int skip, int limit)
        {

            var result = await _collection.Aggregate()
            .Match(x => x.AccountId == AccountId)
            .AppendStage<BsonDocument>(new BsonDocument
            {
                {
                    "$addFields",new BsonDocument
                    {
                        {
                            "limitLookup",new BsonDocument
                            {
                                {
                                    "$slice",new BsonArray
                                    {
                                        "$Friends",
                                         skip,
                                         limit
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
                    "$lookup", new BsonDocument
                    {
                        {
                            "from",nameof(UserCollection)
                        },
                        {
                            "localField","Friends._id"
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
                                                "Gender",1
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
            }).Unwind("Users")
            .ReplaceRoot<BsonDocument>("$Users").As<UserConvert>().ToListAsync();


            return result;



        }

        public async Task<List<FriendWaitListResponeModel>> GetInfoFromWatiList(string AccountId, int skip, int limit)
        {
            var result = await _collection.Aggregate()
            .Match(x => x.AccountId == AccountId)
            .AppendStage<BsonDocument>(new BsonDocument
            {
                {
                    "$addFields", new BsonDocument
                    {
                        {
                            "limitLookup", new BsonDocument
                            {
                                {
                                    "$slice", new BsonArray
                                    {
                                        "$WaitingList",
                                        skip,
                                        limit
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
                    "$lookup", new BsonDocument
                    {
                        {
                            "from",nameof(UserCollection)
                        },
                        {
                            "localField","limitLookup"
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
                            "as","Users"
                        }
                    }
                }
            })
            .Unwind("Users")
            .ReplaceRoot<BsonDocument>("$Users")
            .As<FriendWaitListResponeModel>()
            .ToListAsync();


            return result ;
        }

        public async Task AddToWaitlistAsync(string AccountId, string  MyId)
        {
            var filter = Builders<FriendCollection>.Filter.Eq(x=>x.AccountId,AccountId);

            var update = Builders<FriendCollection>.Update.AddToSet(x => x.WaitingList, ObjectId.Parse(MyId)); 

            await _collection!.UpdateOneAsync(filter, update,new UpdateOptions() { IsUpsert=true});
        }

        public async Task<UpdateResult> CancelFriendResquestAsync(string MyId, string CancelId)
        {
            var filter = Builders<FriendCollection>.Filter
                .Where(x=>x.AccountId==CancelId&&x.WaitingList.Any(x=>x.Equals(ObjectId.Parse(MyId))));

            var update = Builders<FriendCollection>.Update.Pull(x=>x.WaitingList, ObjectId.Parse(MyId));

            return await _collection!.UpdateOneAsync(filter, update);
        }

        public async Task<List<SearchFriendResponeModel>> FriendSearchAsync(string name, string Id)
        {


            var aggry = await _collection.Aggregate()
                .Match(x => x.AccountId == Id)
                .AppendStage<BsonDocument>(new BsonDocument
                {
                    {
                        "$lookup", new BsonDocument
                        {
                            {
                                "from",nameof(UserCollection)
                            },
                            {
                                "localField", "Friends._id"
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
                                            "$match", new BsonDocument
                                            {
                                                {
                                                    "FullName", new BsonDocument
                                                    {
                                                        {
                                                            "$regex",new Regex($"^{name}")
                                                        },
                                                        {
                                                            "$options","i"
                                                        }

                                                    }
                                                }

                                            }
                                        }
                                    },
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
                                                    "Gender",1
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
                
                .Unwind("Users").ReplaceRoot<BsonDocument>("$Users").As<SearchFriendResponeModel>().ToListAsync();


            Debug.WriteLine(aggry);


            return aggry;
        }

        public async Task<UpdateResult> RejectFriendRequest(string AccountId, string RejectId)
        {
            var filter = Builders<FriendCollection>
                    .Filter
                    .Eq(x => x.AccountId, AccountId);

            var update = Builders<FriendCollection>
            .Update
                .Pull(x => x.WaitingList, ObjectId.Parse(RejectId));

            return await _collection!.UpdateOneAsync(filter, update);

        }

        public async Task<List<UserConvert>?> GetFriendNotInGroup(string MyId, string GroupId,int skip,int limit)
        {
            var aggry = await _collection.Aggregate()
                .Match(x => x.AccountId == MyId)
                .AppendStage<BsonDocument>(new BsonDocument
                {
                    {
                        "$lookup", new BsonDocument
                        {
                            {
                                "from",nameof(ConversationCollection)
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
                                                    "_id",ObjectId.Parse(GroupId)
                                                }
                                            }
                                        }
                                    },
                                    new BsonDocument
                                    {
                                        {
                                            "$project", new BsonDocument
                                            {
                                                {
                                                    "_id",0
                                                },
                                                {
                                                    "Owners",1
                                                }
                                            }
                                        }
                                    },

                                }
                            },
                            {
                                "as","Members"
                            }
                        }
                    }
                }).AppendStage<BsonDocument>(new BsonDocument
                {
                    {
                        "$addFields", new BsonDocument
                        {
                            {
                                "Member", new BsonDocument
                                {
                                    {
                                        "$first","$Members"
                                    }
                                }
                            }
                        }
                    }
                }).
                AppendStage<BsonDocument>(new BsonDocument
                {
                    {
                        "$addFields", new BsonDocument
                        {
                            {
                                "idsUser", new BsonDocument
                                {
                                    {
                                        "$filter", new BsonDocument
                                        {
                                            {
                                                "input","$Friends"
                                            },
                                            {
                                                "as","item"
                                            },
                                            {
                                                "cond", new BsonDocument
                                                {
                                                    {
                                                        "$cond", new BsonArray
                                                        {
                                                            new BsonDocument
                                                            {
                                                                {
                                                                    "$in", new BsonArray
                                                                    {
                                                                        "$$item._id",
                                                                        "$Member.Owners"
                                                                    }
                                                                }
                                                            },
                                                            "$$REMOVE",
                                                            "$$item"
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
                }).AppendStage<BsonDocument>(new BsonDocument
                {
                    {
                        "$lookup", new BsonDocument
                        {
                            {
                                "from", nameof(UserCollection)
                            },
                            {
                                "localField","idsUser._id"
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
                                            "$skip",skip
                                        }
                                    },
                                    new BsonDocument
                                    {
                                        {
                                            "$limit",limit
                                        }
                                    },
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
                                                    "Gender",1
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
                        "Users",1
                    }
                })
            .Unwind("Users").ReplaceRoot<BsonDocument>("$Users").As<UserConvert>().ToListAsync();


            Debug.WriteLine(aggry);
            return aggry;
        }
    }
}
