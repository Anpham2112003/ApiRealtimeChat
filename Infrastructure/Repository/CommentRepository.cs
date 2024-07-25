using Domain.Entities;
using Domain.ResponeModel;
using Infrastructure.MongoDBContext;
using Infrastructure.Repository.BaseRepository;
using MongoDB.Bson;
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
        public CommentRepository(IMongoDB mongoDB) : base(mongoDB)
        {
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

        public async Task<UpdateResult> RepComment(string AccountId, string PostId, string ParentId, Comment comment)
        {
            var builder = Builders<PostCollection>.Filter;

            var filter = Builders<PostCollection>.Filter.And(
                    builder.Eq(x=>x.AccountId,AccountId),

                    builder.ElemMatch(x => x.Posts,Builders<Post>.Filter.And(
                            Builders<Post>.Filter.Eq(x=>x.Id,PostId),
                            Builders<Post>.Filter.Eq(x=>x.AllowComment,true)
                    ))
            );

            var update = Builders<PostCollection>.Update
                .Push("Posts.$[p].Comments", comment);

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
    }
}
