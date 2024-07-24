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

        public IUserRepository userRepository 

            => _userRepository ??= new UserRepository(_mongoDB);

        private IUserRepository? _userRepository;


        public IFriendRepository friendRepository 

            => _friendRepository ??= new FriendRepository(_mongoDB);

        private IFriendRepository? _friendRepository;

        public IGroupRepository groupRepository

            => _groupRepository ??= new GroupRepository(_mongoDB);

        private IGroupRepository? _groupRepository;


       

        public IConversationRepository conversationRepository

            => _conversationRepository ??= new ConversationRepository(_mongoDB);

        private  IConversationRepository? _conversationRepository;
        
        public IMessageRepository messageRepository

            =>_messageRepository ??= new MessageRepository(_mongoDB);

        private IMessageRepository? _messageRepository;

        public INotificationRepository notificationRepository

            => _notificationRepository ??= new NotificationRepository(_mongoDB);

        private INotificationRepository _notificationRepository;

        public IPostRepository postRepository 

            => _postRepository ??= new PostRepository(_mongoDB);

        private IPostRepository? _postRepository;

        public ICommentRepository commentRepository 

            => _commentRepository ??= new CommentRepository(_mongoDB);

        private ICommentRepository? _commentRepository;


        public async Task<IClientSessionHandle> TransactionAsync()
        {
           return await _mongoDB.GetClient().StartSessionAsync();
        }

        

        public void Dispose()
        {
            
        }
    }
}
