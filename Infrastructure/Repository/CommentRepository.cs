using Domain.Entities;
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
    public class CommentRepository : AbstractRepository<CommentCollection>, ICommentRepository
    {
        public CommentRepository(IMongoDB mongoDB) : base(mongoDB)
        {
            _collection=mongoDB.GetCollection<CommentCollection>(nameof(CommentCollection));    
        }

        public async Task RemoveCommentCollection(string PostId)
        {
            var filter = Builders<CommentCollection>.Filter.Eq(x=>x.PostId,PostId);

            await _collection!.DeleteOneAsync(filter);
        }

        public async Task<UpdateResult> PushComment(string PostId,Comment comment)
        {
            var filter = Builders<CommentCollection>.Filter.And(
                    Builders<CommentCollection>.Filter.Eq(x => x.PostId, PostId),
                    Builders<CommentCollection>.Filter.Eq(x => x.AllowComment, true)
            );

            var update = Builders<CommentCollection>.Update
                .Inc(x=>x.TotalComment,1)
                .Push(x => x.Comments, comment);

            return await _collection!.UpdateOneAsync(filter, update);
        }

        public async Task<UpdateResult> RepComment(string PostId,string ParentId,Comment comment)
        {
            var filter = Builders<CommentCollection>.Filter.And(
                    Builders<CommentCollection>.Filter.Eq(x => x.PostId, PostId),
                    Builders<CommentCollection>.Filter.Eq(x=>x.AllowComment,true),
                    Builders<CommentCollection>.Filter.Eq("Comments._id", ObjectId.Parse(ParentId))
            );
            var update = Builders<CommentCollection>.Update
                .Inc("Comments.$[m].TotalChildComment",1)
                .Push(x=>x.Comments, comment);

            var arrayFilter = new[]
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument
                {
                    {
                        "m._id",ObjectId.Parse(ParentId)
                    }
                })
            };

            return await _collection!.UpdateOneAsync(filter,update,new UpdateOptions { ArrayFilters=arrayFilter});
        }

        public async Task<CommentCollection> GetCommandPost(string PostId,int skip,int limit)
        {
            var result = _collection.Aggregate()
                        .Match(x=>x.PostId==PostId)
                        .AppendStage<BsonDocument>(new BsonDocument
                        {
                            {
                                "$addFields",new BsonDocument
                                {
                                    {
                                        "filterComment",new BsonDocument
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
                                            },
                                            {
                                                "$skip",skip
                                            },
                                            {
                                                "$limit",limit
                                            }
                                        }

                                    }
                                }
                            }
                        })
        }
    }
}
