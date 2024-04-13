using Domain.Settings;
using Infrastructure.MongoDBContext;
using Infrastructure.Repository;
using Infrastructure.Repository.BaseRepository;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Unit0fWork
{
    public class UnitOfWork : IUnitOfWork
    {
       
        private readonly IMongoDB _mongoDB;

        public UnitOfWork(IMongoDB mongoDB)
        {
            _mongoDB = mongoDB;
        }

        public IAccountRepository accountRepository 

            =>   _accountRepository ??= new AccountRepository(_mongoDB);
            

        private IAccountRepository? _accountRepository;

        public async Task<IClientSessionHandle> TransactionAsync()
        {
           return await _mongoDB.GetClient().StartSessionAsync();
        }

        

        public void Dispose()
        {
            
        }
    }
}
