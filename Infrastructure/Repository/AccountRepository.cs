using Domain.Entites;
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
      
            await base._collection.FindOneAndUpdateAsync(filter, update);
        }

        public async Task<bool> CheckAccountExist(ObjectId id)
        {
            var filter= Builders<AccountCollection>
                .Filter.Eq(x=>x.Id, id);

            var fields = Builders<AccountCollection>
                .Projection.Include(x => x.Id);
            
            var result = await _collection.Find(filter)
                .Project<AccountCollection>(fields)
                .FirstOrDefaultAsync();

           return result is null ? false: true;
        }
    }
}
