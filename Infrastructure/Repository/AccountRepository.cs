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
    public class AccountRepository : AbstractRepository<AccountCollection>,IAccountRepository
    {
        
        public AccountRepository(IMongoDB mongoDB) : base(mongoDB)
        {
            base._collection = mongoDB.GetCollection<AccountCollection>(nameof(AccountCollection));

            var index = Builders<AccountCollection>.IndexKeys.Ascending(x => x.Email);

            _collection.Indexes.CreateOne(new CreateIndexModel<AccountCollection>(index, new CreateIndexOptions { Unique = true }));
        }

        public async Task<AccountCollection?> FindAccountByEmail(string email)
        {

            var query =  base._collection.AsQueryable();

            var result = await query.FirstOrDefaultAsync(x=> x.Email == email);

            return result;
        }

        public async Task SoftDeleteAccount(AccountCollection account)
        {
            var filter = Builders<AccountCollection>.Filter.Eq(x => x.Email, account.Email);

            var update = Builders<AccountCollection>.Update
                .Set(x=>x.IsDelete,true)
                .Set(x=>x.DeletedAt,DateTime.UtcNow);
      
            await base._collection!.UpdateOneAsync(filter, update);
        }

      

        public async Task<AccountInformationResponseModel?> GetAccountInformationAsync(string Email)
        {
            
            var result = await _collection.Aggregate()
                .Match(x=>x.Email==Email)
                .AppendStage<BsonDocument>(new BsonDocument
                {
                    {
                        "$lookup", new BsonDocument
                        {
                            {
                                "from",nameof(UserCollection)
                            },
                            {
                                "localField","_id"
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
                                "as","User"
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
                        "Email",1
                    },
                    {
                        "Password",1
                    },
                    {
                        "IsDelete",1
                    },
                    {
                        "User",new BsonDocument
                        {
                            {
                                "$arrayElemAt", new BsonArray
                                {
                                    "$User",-1
                                }
                            }
                        }
                    }
                }).As<AccountInformationResponseModel>()
                .FirstOrDefaultAsync();

            return result;
        }

        public async Task<UpdateResult> UpdateTokenUser(string Id, string Token)
        {
            var filter = Builders<AccountCollection>.Filter.Eq(x=>x.Id, Id);

            var update = Builders<AccountCollection>.Update.Set(x=>x.ReFreshToken, Token);

            return await _collection!.UpdateOneAsync(filter,update);
        }

        public async Task UpdatePassword(string Email, string Password)
        {
            var filter = Builders<AccountCollection>.Filter.Eq(x => x.Email, Email);

            var update = Builders<AccountCollection>.Update.Set(x => x.Password, Password);

            await _collection!.UpdateOneAsync(filter, update);
        }
    }
}
