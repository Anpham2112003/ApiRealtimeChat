using Domain.Entities;
using Domain.ResponeModel;
using MongoDB.Bson;
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
        public Task<AccountCollection?> FindAccountByEmail(string email);
        public Task SoftDeleteAccount(AccountCollection account);
        public  Task<AccountInformationResponseModel?> GetAccountInformationAsync(string Email);
        public  Task<UpdateResult> UpdateTokenUser(string Id, string Token);
        public  Task UpdatePassword(string Email, string Password);

    }
}
