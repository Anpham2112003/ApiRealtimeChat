using Domain.Entities;
using Domain.Enums;
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
            Debug.WriteLine(result);
        }
    }
}
