using Domain.Entities;
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
    public class FriendRepository : AbstractRepository<FriendCollection>, IFriendRepository
    {
        private IMongoCollection<UserCollection>? _userCollection;
        public FriendRepository(IMongoDB mongoDB) : base(mongoDB)
        {
            base._collection = mongoDB.GetCollection<FriendCollection>(nameof(FriendCollection));

            var AccountIdIndex = Builders<FriendCollection>.IndexKeys.Ascending(x => x.AccountId);
            
            _collection.Indexes.CreateOne(new CreateIndexModel<FriendCollection>(AccountIdIndex, new CreateIndexOptions { Unique=true}));
        }

        public async Task AddFriendAsync(string AccountId, string FriendId)
        {
            var fillter = Builders<FriendCollection>
                .Filter.And(
                    Builders<FriendCollection>.Filter.Eq(x => x.AccountId, AccountId),
                    Builders<FriendCollection>.Filter.Ne("Friends._id", ObjectId.Parse(FriendId))
                );

            var update = Builders<FriendCollection>
                .Update
                .AddToSet(x => x.Friends, new Friend(FriendId));

            await _collection!.UpdateOneAsync(fillter, update, new UpdateOptions() { IsUpsert = true });
        }

        public async Task AcceptFriend(string myId, string WaitListId)
        {
            var filter = Builders<FriendCollection>.Filter.Eq(x => x.AccountId, myId);

            var update = Builders<FriendCollection>

                .Update
                .Pull(x => x.WaitingList, ObjectId.Parse(WaitListId))
                .AddToSet(x => x.Friends, new Friend(WaitListId));

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

        public async Task<GetFriendsByAccountConvert?> GetFriendAysnc(string AccountId, int skip, int limit)
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

        public async Task<GetInfoWaitAccecptConvert?> GetInfoFromWatiList(string AccountId, int skip, int limit)
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
                                        "$Waitlist",
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
                { "_id", 0 },

                {

                    "Result", 1
                }



            }).As<GetInfoWaitAccecptConvert>().FirstOrDefaultAsync();

            return result;
        }

        public async Task AddToWaitlistAsync(string AccountId, string  WaitListId)
        {
            var filter = Builders<FriendCollection>.Filter.Eq(x=>x.AccountId,AccountId);

            var update = Builders<FriendCollection>.Update.Push(x => x.WaitingList, ObjectId.Parse(WaitListId));

            await _collection!.UpdateOneAsync(filter, update,new UpdateOptions() { IsUpsert=true});
        }

        public async Task<UpdateResult> CancelFriendResquestAsync(string MyId, string CancelId)
        {
            var filter = Builders<FriendCollection>.Filter
                .Where(x=>x.Id==CancelId&&x.WaitingList.Any(x=>x.Equals(ObjectId.Parse(MyId))));

            var update = Builders<FriendCollection>.Update.Pull(x=>x.WaitingList, ObjectId.Parse(MyId));

            return await _collection!.UpdateOneAsync(filter, update);
        }
    }
}
