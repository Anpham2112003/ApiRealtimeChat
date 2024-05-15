using Domain.Entites;
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
    public class UserRepository : AbstractRepository<UserCollection>, IUserRepository
    {
        public UserRepository(IMongoDB mongoDB) : base(mongoDB)
        {
            base._collection=mongoDB.GetCollection<UserCollection>(nameof(UserCollection));
        }

        public async Task<UserCollection?> FindUserByAccountId(ObjectId id)
        {
            var result = await _collection.FindAsync(x => x.AccountId== id);

            return await result.SingleOrDefaultAsync();
        }

        public async Task InsertUserAsync(UserCollection user)
        {
            await base._collection!.InsertOneAsync(user);
        }

        public async Task ChangeStateUserAsync(ObjectId AccountId)
        {
            var filter = Builders<UserCollection>.Filter.Eq(x=>x.Id, AccountId);
            var update = Builders<UserCollection>.Update.Set(x => x.State, Domain.Enums.UserState.Online);
            await _collection!.UpdateOneAsync(filter, update);
        }
    }
}
