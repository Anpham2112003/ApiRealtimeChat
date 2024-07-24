using Domain.Entities;
using Domain.Enums;
using Domain.ResponeModel;
using Infrastructure.MongoDBContext;
using Infrastructure.Repository.BaseRepository;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using MongoDB.Bson;
using MongoDB.Driver;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class UserRepository : AbstractRepository<UserCollection>, IUserRepository
    {
        public UserRepository(IMongoDB mongoDB) : base(mongoDB)
        {
            base._collection=mongoDB.GetCollection<UserCollection>(nameof(UserCollection));

            var AccountIdIndex = Builders<UserCollection>.IndexKeys.Ascending(x => x.AccountId);
            var fullNameIndex= Builders<UserCollection>.IndexKeys.Text(x => x.FullName);

            var indexs = new List<CreateIndexModel<UserCollection>>()
            {
                new CreateIndexModel<UserCollection>(AccountIdIndex,new CreateIndexOptions<UserCollection>{Unique=true} ),
                new CreateIndexModel<UserCollection>(fullNameIndex)
            };

            _collection.Indexes.CreateMany(indexs);

        }

        public async Task<UserCollection?> FindUserByAccountId(string id)
        {
            var result = await _collection.FindAsync(x => x.AccountId== id);

            return await result.SingleOrDefaultAsync();
        }

        public async Task InsertUserAsync(UserCollection user)
        {
            await base._collection!.InsertOneAsync(user);
        }

        public async Task ChangeStateUserAsync(string AccountId, UserState state)
        {
            var filter = Builders<UserCollection>.Filter.Eq(x=>x.AccountId, AccountId);

            var update = Builders<UserCollection>.Update.Set(x=>x.State, state);

            var result =   await _collection!.UpdateOneAsync(filter, update);
            
        }

        public async Task UpdateAvatarUser(string AccountId, string avatarUrl)
        {
            var filter = Builders<UserCollection>.Filter.Eq(x => x.AccountId,AccountId);

            var update = Builders<UserCollection>.Update.Set(x => x.Avatar, avatarUrl);

            await _collection!.UpdateOneAsync(filter, update);
        }

        public async Task UpdateProfileUser(string AccountId,BsonDocument document)
        {
            var filter = Builders<UserCollection>.Filter.Eq(x=>x.AccountId,AccountId);

            await _collection!.UpdateOneAsync(filter, document);
        }

        public async Task RemoveAvatarUser(string AccountId)
        {
            var filter = Builders<UserCollection>.Filter.Eq(x => x.AccountId, AccountId);

            var update = Builders<UserCollection>.Update.Set(x => x.Avatar, null);

            await _collection!.UpdateOneAsync(filter,update);
        }

        public async Task<List<UserResponseModel>> SearchUser(string name)
        {
            var filter = Builders<UserCollection>.Filter.Regex(x => x.FullName, new Regex($"^{name}"));

            var project = Builders<UserCollection>.Projection.Expression(x => new UserResponseModel
            {
                AccountId = x.AccountId,
                Avatar = x.Avatar,
                FullName = x.FullName,
                State = x.State,
            });
             
               
           

            return await _collection.Find(filter).Project<UserResponseModel>(project).ToListAsync();
        }

        public async Task<ViewProfileResponeModel?> ViewProfileUser(string Id, string UserId)
        {
            var aggry = await _collection.Aggregate()
                .Match(x => x.AccountId == Id)
                .AppendStage<BsonDocument>(new BsonDocument
                {
                    {
                        "$lookup", new BsonDocument
                        {
                            {
                                "from",nameof(FriendCollection)
                            },
                            {
                                "localField","AccountId"
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
                                                    "IsFriend", new BsonDocument
                                                    {
                                                        {
                                                            "$in", new BsonArray
                                                            {
                                                                UserId,
                                                                "$Friends._id"
                                                            }
                                                        }
                                                    }
                                                },
                                                {
                                                    "Invited",new BsonDocument
                                                    {
                                                        {
                                                            "$in", new BsonArray
                                                            {
                                                                UserId,
                                                                "$WaitList"
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
                                "as","Result"
                            }
                        }
                    }
                }).AppendStage<BsonDocument>(new BsonDocument
                {
                    {
                        "$replaceRoot", new BsonDocument
                        {
                            {
                                "newRoot", new BsonDocument
                                {
                                    {
                                        "$mergeObjects", new BsonArray
                                        {
                                            new BsonDocument
                                            {
                                                {
                                                    "$arrayElemAt", new BsonArray
                                                    {
                                                    "$Result",0
                                                    }
                                                }
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
                        "_id",0
                    },
                    {
                        "Result",0
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
                }).As<ViewProfileResponeModel>().FirstOrDefaultAsync();

            return aggry;

        }
    }
}
