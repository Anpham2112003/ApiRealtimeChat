using Domain.Entities;
using Domain.ResponeModel;
using Infrastructure.MongoDBContext;
using Infrastructure.Repository.BaseRepository;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class CommentRepository : AbstractRepository<PostCollection>, ICommentRepository
    {
        private IMongoCollection<BsonDocument> _bsonCollection;
        public CommentRepository(IMongoDB mongoDB) : base(mongoDB)
        {
            _bsonCollection=mongoDB.GetCollection<BsonDocument>(nameof(PostCollection));

            _collection=mongoDB.GetCollection<PostCollection>(nameof(PostCollection));    
        }

        public async Task<UpdateResult> PushComment(string AccountId, string PostId, Comment comment)
        {
            var builder = Builders<PostCollection>.Filter;

            var filter = Builders<PostCollection>.Filter.And(
                    builder.Eq(x => x.AccountId, AccountId),

                    builder.ElemMatch(x => x.Posts,

                          Builders<Post>.Filter.And(
                            Builders<Post>.Filter.Eq(x=>x.Id, PostId),
                            Builders<Post>.Filter.Eq(x=>x.AllowComment,true)
                          )
                    )
            );

            var update = Builders<PostCollection>.Update
                .Inc("LatestPosts.$[p].TotalComment",1)
                .Push("LatestPosts.$[p].Comments",comment)
                .Inc(x=>x.Posts.FirstMatchingElement().TotalComment,1)
                .Push(x => x.Posts.FirstMatchingElement().Comments, comment);

            var arrayFilter = new[]
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument
                {
                    {
                        "p._id",ObjectId.Parse(PostId)
                    }
                })
            };

            return await _collection!.UpdateOneAsync(filter,update, new UpdateOptions { ArrayFilters=arrayFilter});
        }

        public async Task<BulkWriteResult> RepComment(string AccountId, string PostId, string ParentId, Comment comment)
        {
            var builder = Builders<PostCollection>.Filter;
            var updateBuilder= Builders<PostCollection>.Update;

            var filter = Builders<PostCollection>.Filter.And(
                    builder.Eq(x=>x.AccountId,AccountId),

                    builder.ElemMatch(x => x.Posts,Builders<Post>.Filter.And(
                            Builders<Post>.Filter.Eq(x=>x.Id,PostId),
                            Builders<Post>.Filter.Eq(x=>x.AllowComment,true)
                    ))
            );

            var PostFilter = new[]
             {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument
                {
                    {
                        "p._id",ObjectId.Parse(PostId)
                    }
                }),

            };

            var CommentFilter = new[]
             {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument
                {
                    {
                        "p._id",ObjectId.Parse(PostId)
                    }
                }),

                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument
                {
                    {
                        "c._id",ObjectId.Parse(comment.ParentId)
                    }
                })
            };


            var bulket = new WriteModel<PostCollection>[]
            {
               new UpdateOneModel<PostCollection>(filter,updateBuilder.Inc("Posts.$[p].TotalComment",1)){ArrayFilters= PostFilter },

               new UpdateOneModel<PostCollection>(filter,updateBuilder.Push("Posts.$[p].Comments",comment)){ArrayFilters= PostFilter },

               new UpdateOneModel<PostCollection>(filter,updateBuilder.Inc("Posts.$[p].Comments.$[c].TotalChildComment",1)){ArrayFilters= CommentFilter },
            };
                

            

            return await _collection!.BulkWriteAsync(bulket);
        }

        public async Task<UpdateResult> BlockComment(string AccountId,string PostId)
        {
            
            var filter = Builders<PostCollection>.Filter.Eq(x=>x.AccountId, AccountId);

            var update = Builders<PostCollection>.Update
                .Set("LatestPosts.$[p].AllowComment", false)
                .Set("Posts.$[p].AllowComment", false);

            var arrayFilter = new[]
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument
                {
                    {
                        "p._id",ObjectId.Parse(PostId)
                    }
                })
            };

            return await _collection!.UpdateOneAsync(filter,update,new UpdateOptions { ArrayFilters=arrayFilter});
        }


        public async Task<UpdateResult> UnBlockComment(string AccountId, string PostId)
        {

            var filter = Builders<PostCollection>.Filter.Eq(x => x.AccountId, AccountId);

            var update = Builders<PostCollection>.Update
                .Set("LatestPosts.$[p].AllowComment", true)
                .Set("Posts.$[p].AllowComment", true);

            var arrayFilter = new[]
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument
                {
                    {
                        "p._id",ObjectId.Parse(PostId)
                    }
                })
            };

            return await _collection!.UpdateOneAsync(filter, update, new UpdateOptions { ArrayFilters = arrayFilter });
        }

        public async Task<UpdateResult> HiddenComment(string AccountId, string PostId)
        {

            var filter = Builders<PostCollection>.Filter.Eq(x => x.AccountId, AccountId);

            var update = Builders<PostCollection>.Update
                .Set("LatestPosts.$[p].HiddenComment", true)
                .Set("Posts.$[p].HiddenComment", true);

            var arrayFilter = new[]
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument
                {
                    {
                        "p._id",ObjectId.Parse(PostId)
                    }
                })
            };

            return await _collection!.UpdateOneAsync(filter, update, new UpdateOptions { ArrayFilters = arrayFilter });
        }

        public async Task<UpdateResult> UnHiddenComment(string AccountId, string PostId)
        {

            var filter = Builders<PostCollection>.Filter.Eq(x => x.AccountId, AccountId);

            var update = Builders<PostCollection>.Update
                .Set("LatestPosts.$[p].HiddenComment", false)
                .Set("Posts.$[p].HiddenComment",false);

            var arrayFilter = new[]
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument
                {
                    {
                        "p._id",ObjectId.Parse(PostId)
                    }
                })
            };

            return await _collection!.UpdateOneAsync(filter, update, new UpdateOptions { ArrayFilters = arrayFilter });
        }

        public async Task<IEnumerable<CommentResponseModel>> GetCommentPost(string AccountId, string PostId,int skip,int limit)
        {
            var aggry =await _collection.Aggregate()
                .Match(x => x.AccountId == AccountId)
                .AppendStage<BsonDocument>(new BsonDocument
                {
                    {
                        "$addFields",new BsonDocument
                        {
                            {
                                "FindPost",new BsonDocument
                                {
                                    {
                                        "$arrayElemAt",new BsonArray
                                        {
                                            "$Posts",

                                            new BsonDocument
                                            {
                                                {
                                                    "$indexOfArray",new BsonArray
                                                    {
                                                        "$Posts._id",ObjectId.Parse(PostId)
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
                .Project<BsonDocument>(new BsonDocument
                {
                    {
                        "_id",0
                    },
                    {
                        "FindPost",1
                    }
                })
                .ReplaceRoot<BsonDocument>("$FindPost")
                .AppendStage<BsonDocument>(new BsonDocument
                {
                    {
                        "$addFields",new BsonDocument
                        {
                            {
                                "FilterComment",new BsonDocument
                                {
                                    {
                                        "$cond",new BsonArray
                                        {
                                            new BsonDocument
                                            {
                                                {
                                                    "$eq",new BsonArray
                                                    {
                                                        "$HiddenComment",false
                                                    }
                                                }
                                            },
                                            new BsonDocument
                                            {
                                                {
                                                    "$filter",new BsonDocument
                                                    {
                                                        {
                                                            "input","$Comments"
                                                        },
                                                        {
                                                            "as","item"
                                                        },
                                                        {
                                                            "cond",new BsonDocument
                                                            {
                                                                {
                                                                    "$eq",new BsonArray
                                                                    {
                                                                        "$$item.ParentId",BsonNull.Value
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            },

                                            BsonNull.Value
                                            
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
                        "_id",0
                    },
                    {
                        "Comments",new BsonDocument
                        {
                            {
                                "$slice",new BsonArray
                                {
                                    "$FilterComment",skip,limit
                                }
                            }
                        }
                    }
                }).Unwind("Comments")
                .ReplaceRoot<BsonDocument>("$Comments")
                .AppendStage<BsonDocument>(new BsonDocument
                {
                    {
                        "$lookup",new BsonDocument
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
                })
                .ReplaceRoot<BsonDocument>(new BsonDocument
                {
                    
                     {
                          "$mergeObjects",new BsonArray
                          {
                               new BsonDocument
                               {
                                   {
                                       "$arrayElemAt",new BsonArray
                                       {
                                                "$Users",0
                                       }
                                   }
                               },

                              "$$ROOT"
                          }
                     }
                    
                })
                .Project(new BsonDocument
                {
                    {
                        "Users",0
                    }
                }).As<CommentResponseModel>().ToListAsync();

            return aggry;
        }
        public async Task<IEnumerable<CommentResponseModel>> GetRepComment(string AccountId, string PostId,string ParentId, int skip, int limit)
        {
            var aggry = await _collection.Aggregate()
                .Match(x => x.AccountId == AccountId)
                .AppendStage<BsonDocument>(new BsonDocument
                {
                    {
                        "$addFields",new BsonDocument
                        {
                            {
                                "FindPost",new BsonDocument
                                {
                                    {
                                        "$arrayElemAt",new BsonArray
                                        {
                                            "$Posts",

                                            new BsonDocument
                                            {
                                                {
                                                    "$indexOfArray",new BsonArray
                                                    {
                                                        "$Posts._id",ObjectId.Parse(PostId)
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
                .Project<BsonDocument>(new BsonDocument
                {
                    {
                        "_id",0
                    },
                    {
                        "FindPost",1
                    }
                })
                .ReplaceRoot<BsonDocument>("$FindPost")
                .AppendStage<BsonDocument>(new BsonDocument
                {
                    {
                        "$addFields",new BsonDocument
                        {
                            {
                                "FilterComment",new BsonDocument
                                {
                                    {
                                        "$cond",new BsonArray
                                        {
                                            new BsonDocument
                                            {
                                                {
                                                    "$eq",new BsonArray
                                                    {
                                                        "$HiddenComment",false
                                                    }
                                                }
                                            },
                                            new BsonDocument
                                            {
                                                {
                                                    "$filter",new BsonDocument
                                                    {
                                                        {
                                                            "input","$Comments"
                                                        },
                                                        {
                                                            "as","item"
                                                        },
                                                        {
                                                            "cond",new BsonDocument
                                                            {
                                                                {
                                                                    "$eq",new BsonArray
                                                                    {
                                                                        "$$item.ParentId",ObjectId.Parse(ParentId)
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            },

                                            BsonNull.Value

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
                        "_id",0
                    },
                    {
                        "Comments",new BsonDocument
                        {
                            {
                                "$slice",new BsonArray
                                {
                                    "$FilterComment",skip,limit
                                }
                            }
                        }
                    }
                }).Unwind("Comments")
                .ReplaceRoot<BsonDocument>("$Comments")
                .AppendStage<BsonDocument>(new BsonDocument
                {
                    {
                        "$lookup",new BsonDocument
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
                })
                .ReplaceRoot<BsonDocument>(new BsonDocument
                {

                     {
                          "$mergeObjects",new BsonArray
                          {
                               new BsonDocument
                               {
                                   {
                                       "$arrayElemAt",new BsonArray
                                       {
                                                "$Users",0
                                       }
                                   }
                               },

                              "$$ROOT"
                          }
                     }

                })
                .Project(new BsonDocument
                {
                    {
                        "Users",0
                    }
                }).As<CommentResponseModel>().ToListAsync();

            return aggry;
        }

        public async Task<BulkWriteResult>RemoveComment(string AccountId,string MyId,string PostId,string CommentId,string ParentId)
        {

            var builder = Builders<BsonDocument>.Filter;

            var findChildComment = Builders<BsonDocument>.Filter

           .ElemMatch<BsonValue>("Posts.Comments", new BsonDocument
                     {
                        {
                            "_id",ObjectId.Parse(CommentId)
                        },
                        {
                            "AccountId",ObjectId.Parse(MyId)
                        }
           });

            var findParentComment = Builders<BsonDocument>.Filter


             .ElemMatch<BsonValue>("Posts.Comments", new BsonDocument
                     {
                        {
                            "_id",ObjectId.Parse(ParentId)
                        }
             });
          


            var childCommentFilter = new[]
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument
                {
                    {
                        "p._id",ObjectId.Parse(PostId)
                    }
                }),

            };

            var parentCommentFilter = new[]
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument
                {
                    {
                        "p._id",ObjectId.Parse(PostId)
                    },
                    
                }),
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument
                {
                    {
                        "c._id",ObjectId.Parse(ParentId)
                    },

                }),

            };




            var pullChildComment = Builders<BsonDocument>.Update
                .Inc("Posts.$[p].TotalComment", -1)
                .PullFilter("Posts.$[p].Comments", Builders<Comment>.Filter.Eq(x=>x.Id,CommentId));

            var updateParentComment = Builders<BsonDocument>.Update
                .Inc("Posts.$[p].Comments.$[c].TotalChildComment", -1);


            var bulket = new WriteModel<BsonDocument>[]
            {
                new UpdateOneModel<BsonDocument>(findChildComment,pullChildComment){ArrayFilters=childCommentFilter},

                new UpdateOneModel<BsonDocument>(findParentComment,updateParentComment){ArrayFilters=parentCommentFilter},
            };


            

            return await _bsonCollection!.BulkWriteAsync(bulket);


        }

    }
}
