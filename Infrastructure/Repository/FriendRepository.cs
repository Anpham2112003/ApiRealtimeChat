using Domain.Entites;
using Domain.ResponeModel.BsonConvert;
using Infrastructure.MongoDBContext;
using Infrastructure.Repository.BaseRepository;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class FriendRepository : AbstractRepository<FriendCollection>,IFriendRepository
    {
        private IMongoCollection<UserCollection> _userCollection;
        public FriendRepository(IMongoDB mongoDB) : base(mongoDB)
        {
            base._collection=mongoDB.GetCollection<FriendCollection>(nameof(FriendCollection));
            _userCollection=mongoDB.GetCollection<UserCollection>(nameof(UserCollection));  
        }

       public async Task AddFriendAsync(ObjectId AccountId, ObjectId FriendId)
        {
            var fillter = Builders<FriendCollection>
                .Filter.Eq(x => x.AccountId, AccountId);

            var update = Builders<FriendCollection>
                .Update
                .AddToSet(x => x.Friends, new Friend(FriendId));

            await _collection.UpdateOneAsync(fillter,update, new UpdateOptions() { IsUpsert=true});
        }

        public async Task<UpdateResult> RemoveFriendAsync(ObjectId AccountId ,  ObjectId FriendId)
        {
            var filter = Builders<FriendCollection>
                .Filter.Eq(x => x.AccountId, AccountId);

            var update = Builders<FriendCollection>
                .Update
                .Pull(x => x.Friends, new Friend(FriendId));

            var result =   await _collection.UpdateOneAsync(filter, update);

            return result;
        }

        public async Task<GetFriendsByAccountConvert?> GetFriendAysnc(ObjectId AccountId , int skip,int limit)
        {

            var result =await  _collection.Aggregate()
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
            .Lookup(nameof(UserCollection), "limitLookup._id", "AccountId", "Result")
            .AppendStage<BsonDocument>(new BsonDocument
            {
                {
                    "$unset", new BsonArray
                    {
                        "Result._id",
                        "Result.FistName",
                        "Result.LastName",
                        "Result.UpdatedAt"
                    }
                }
            })

            .Project(new BsonDocument
            {
                {"_id",0 }, 

                {
                   
                    "Result",1
                }
                


            }).As<GetFriendsByAccountConvert>()

            .FirstOrDefaultAsync();
            Debug.WriteLine(result);
           
          
            return result;
           
                
               
        }

        
    }
}
