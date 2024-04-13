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
        }

        public async Task<AccountCollection> FindAccountByEmail(string email)
        {

            var query =  base._collection.AsQueryable();

            var result = await query.FirstOrDefaultAsync(x=> x.Email == email);

            return result;
        }

        public async Task RemoveAccountByEmail(string email)
        {
            var filter = Builders<AccountCollection>.Filter.Eq(x => x.Email, email);
      
            await base._collection.FindOneAndDeleteAsync(filter);
        }
    }
}
