using Domain.Entites;
using Infrastructure.MongoDBContext;
using Infrastructure.Repository;
using Infrastructure.Repository.BaseRepository;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Unit0fWork
{
    public interface IUnitOfWork:IDisposable
    {
        public Task<IClientSessionHandle> TransactionAsync();
        public IAccountRepository accountRepository { get; }
        public IUserRepository userRepository { get; }
        public IFriendRepository friendRepository { get; }
        public IGroupRepository groupRepository { get; }
        public IMessageRepository messageRepository { get; }
     
    }
}
