using Domain.Entities;
using Domain.ResponeModel;
using Infrastructure.MongoDBContext;
using Infrastructure.Repository.BaseRepository;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
    public class PostRepository : AbstractRepository<PostCollection>, IPostRepository
    {
        private readonly IMongoCollection<FriendCollection> _friendCollection;
        public PostRepository(IMongoDB mongoDB) : base(mongoDB)
        {
            _collection = mongoDB.GetCollection<PostCollection>(nameof(PostCollection)); 

            _friendCollection=mongoDB.GetCollection<FriendCollection>( nameof(FriendCollection));

            var AccountIdx = Builders<PostCollection>.IndexKeys.Ascending(x => x.AccountId);
           
            var TTLIdx = Builders<PostCollection>.IndexKeys.Descending("LatestPost._id");

            var Idsx = new List<CreateIndexModel<PostCollection>>
            {
                new CreateIndexModel<PostCollection>(AccountIdx),
                new CreateIndexModel<PostCollection>(TTLIdx,new CreateIndexOptions{ExpireAfter=TimeSpan.FromDays(3)})
            };

            _collection.Indexes.CreateMany(Idsx);
        }

     
     

        public async Task<UpdateResult> RemovePost(string AccountId,string PostId)
        {
            var filter = Builders<PostCollection>.Filter.Eq(x => x.AccountId, AccountId);


            var update = Builders<PostCollection>.Update
                .PullFilter(x => x.LatestPost, Builders<Post>.Filter.Eq(x => x.Id, PostId))
                .PullFilter(x=>x.Posts, Builders<Post>.Filter.Eq(x=>x.Id,PostId));
           

            return await _collection!.UpdateOneAsync(filter,update);
        }

        public async  Task<UpdateResult> InsertPost(string AccountId,Post post)
        {
            var filter = Builders<PostCollection>.Filter.Eq(x => x.AccountId, AccountId);

            var update = Builders<PostCollection>.Update
                .SetOnInsert(x=>x.CreatedAt,DateTime.UtcNow)
                .Push(x=>x.LatestPost,post)
                .Push(x=>x.Posts, post);
   
          return   await _collection!.UpdateOneAsync(filter,update, new UpdateOptions { IsUpsert=true} );
           
        }


        public async Task<UpdateResult> LikePost(string MyId,string AccountId,string PostId)
        {
            var filter = Builders<PostCollection>.Filter.Eq(x=>x.AccountId, AccountId);

            var update = Builders<PostCollection>.Update
                                .Inc("Posts.$[p].Like",1)
                                .Push("Post.$[p].ListIdLike",ObjectId.Parse(MyId))
                                .Inc("LatestPosts.$[p].Likes",1)
                                .Push("LatestPosts.$[p].ListLike", ObjectId.Parse(MyId));

            var arrayFilter = new[]
            {
               
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument
                {
                    {
                        "p._id",ObjectId.Parse(PostId)
                    }
                }),

            };

            return await _collection!.UpdateOneAsync(filter, update,new UpdateOptions { ArrayFilters=arrayFilter});
        }

        public async Task<UpdateResult> UnLikePost(string MyId, string AccountId,  string PostId)
        {
            var filter = Builders<PostCollection>.Filter.Eq(x => x.AccountId, AccountId);

            var update = Builders<PostCollection>.Update
                                .Inc("LatestPost.$[p].Likes", -1)
                                .Pull("LatestPost.$[p].ListLike", ObjectId.Parse(MyId))
                                .Inc("Posts.$[p].Likes", -1)
                                .Pull("Posts.$[p].ListLike", ObjectId.Parse(MyId));

            var arrayFilter = new[]
            {
               
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument
                {
                    {
                        "p._id",ObjectId.Parse(PostId)
                    }
                }),
            };

            return await _collection!.UpdateOneAsync(filter, update, new UpdateOptions { ArrayFilters=arrayFilter});
        }

        public async Task<IEnumerable<PostResponseModel>> GetLastPostFromListFriend(string AccountId , int skip,int take)
        {
            var result = await _friendCollection.Aggregate()
                    .Match(x => x.AccountId == AccountId)
                    .AppendStage<BsonDocument>(new BsonDocument
                    {
                        {
                            "$lookup", new BsonDocument
                            {
                                {
                                    "from",nameof(PostCollection)
                                },
                                {
                                    "localField","Friends._id"
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
                                                "$project", new BsonDocument
                                                {
                                                    {
                                                        "_id",0

                                                    },
                                                    {
                                                        "LatestPost",new BsonDocument
                                                        {
                                                            {
                                                                "$filter", new BsonDocument
                                                                {
                                                                    {
                                                                        "input","$LatestPost"
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
                                                                                            "$eq", new BsonArray
                                                                                            {
                                                                                                "$$item._id",BsonNull.Value
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
                                        },
                                        new BsonDocument
                                        {
                                            {
                                                "$unwind","$LatestPost"
                                            }
                                        },
                                        new BsonDocument
                                        {
                                            {
                                                "$replaceRoot", new BsonDocument
                                                {
                                                    {
                                                        "newRoot","$LatestPost"
                                                    }
                                                }
                                            }
                                        },

                                        new BsonDocument
                                        {
                                            {
                                                "$skip",skip
                                            },
                                            
                                        },
                                        new BsonDocument
                                        {
                                            {
                                                "$limit",take
                                            }
                                        }

                                    }
                                },

                                {
                                    "as", "Posts"
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
                                    "localField","Posts.AccountId"
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
                    .Project(new BsonDocument
                    {
                        {
                            "_id",0
                        },
                        {
                            "Posts", new BsonDocument
                            {
                                {
                                    "$map",new BsonDocument
                                    {
                                        {
                                            "input","$Posts"
                                        },
                                        {
                                            "as","item"
                                        },
                                        {
                                            "in",new BsonDocument
                                            {
                                                {
                                                    "$mergeObjects", new BsonArray
                                                    {
                                                        new BsonDocument
                                                        {
                                                            {
                                                                "$arrayElemAt", new BsonArray
                                                                {
                                                                    {
                                                                        "$Users"
                                                                    },
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
                                                        },


                                                        "$$item",

                                                        new BsonDocument
                                                        {
                                                            {
                                                                "Liked", new BsonDocument
                                                                {
                                                                    {
                                                                        "$in", new BsonArray
                                                                        {
                                                                            ObjectId.Parse(AccountId),
                                                                            "$$item.ListIdLike"
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
                    .Unwind("Posts")
                    .ReplaceRoot<BsonDocument>("$Posts")
                    .AppendStage<BsonDocument>(new BsonDocument
                    {
                        {
                            "$unset","ListIdLike"
                        }
                    })
                    .As<PostResponseModel>().SortByDescending(x=>x.CreatedAt).ToListAsync();

            Debug.WriteLine(result);

            return result;
        }

        public async Task<IEnumerable<UserResponseModel>> GetListUserLikePost(string AccountId, string PostId,int skip,int take)
        {
            var aggry =await _collection.Aggregate()

                        .Match(x => x.AccountId == AccountId)

                       .AppendStage<BsonDocument>(new BsonDocument
                        {
                            {
                                "$addFields", new BsonDocument
                                {
                                    {
                                        "Post", new BsonDocument
                                        {
                                            {
                                                "$arrayElemAt", new BsonArray
                                                {
                                                    "$Posts",
                                                    new BsonDocument
                                                    {
                                                        {
                                                            "$indexOfArray", new BsonArray
                                                            {
                                                                "$Posts._id", ObjectId.Parse(PostId)
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }).Project(new BsonDocument
                        {
                            {
                                "_id",0
                            },
                            {
                                "Post",1
                            }
                        })
                        .ReplaceRoot<BsonDocument>("$Post")
                        .AppendStage<BsonDocument>(new BsonDocument
                        {
                            {
                                "$lookup", new BsonDocument
                                {
                                    {
                                        "from",nameof(UserCollection)
                                    },
                                    {
                                        "localField","ListIdLike"
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
                                                        },

                                                    }
                                                }
                                            },
                                            new BsonDocument
                                            {
                                                {
                                                    "$skip",skip
                                                }
                                            },
                                            new BsonDocument
                                            {
                                                {
                                                    "$limit",take
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
                        .Unwind("Users")
                        .ReplaceRoot<BsonDocument>("$Users").As<UserResponseModel>().ToListAsync();

            return aggry is null ? Enumerable.Empty<UserResponseModel>():aggry;
        }

        public async Task<IEnumerable<PostResponseModel>> GetPostById(string AccountId, int Skip,int Limit)
        {
            var aggry = await _collection.Aggregate()
                .Match(x => x.AccountId == AccountId)
                .Project(new BsonDocument
                {
                    {
                        "_id",0
                    },
                    {
                        "Posts",new BsonDocument
                        {
                            {
                                "$slice", new BsonArray
                                {
                                    "$Posts",Skip, Limit
                                }
                            }
                        }
                    }
                })
                .Unwind("Posts")
                .ReplaceRoot<BsonDocument>("$Posts")
                .AppendStage<BsonDocument>(new BsonDocument
                {
                    {
                        "$lookup", new BsonDocument
                        {
                            {
                                "from",nameof(UserCollection)
                            },
                            {
                                "localField","AccountId"
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
                .AppendStage<BsonDocument>(new BsonDocument
                {
                    {
                        "$replaceRoot", new BsonDocument
                        {
                            {
                                "newRoot", new BsonDocument
                                {
                                    {
                                        "$mergeObjects",new BsonArray
                                        {
                                            new BsonDocument
                                            {
                                                {
                                                     "$arrayElemAt",new BsonArray
                                                     {
                                                          "$Users",-1
                                                     }
                                                },
                                            },
                                            "$$ROOT"
                                        }
                                    }
                                }
                            }
                        }
                    }
                }).Project(new BsonDocument
                {
                    {
                        "Users",0
                    }
                }).ToListAsync();

            Debug.WriteLine(aggry);

            return aggry is null ? Enumerable.Empty<PostResponseModel>() : Enumerable.Empty<PostResponseModel>();
        }

      

       
    }
}
