using Domain.Entites;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.BaseRepository
{
    public interface IAccountRepository:BaseRepository<AccountCollection>
    {
        public Task<AccountCollection> FindAccountByEmail(string email);
        public Task RemoveAccountByEmail(string email);
    }
}
