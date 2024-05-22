using Domain.Entities;
using Domain.ResponeModel.BsonConvert;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.BaseRepository
{
    public interface IFriendRepository :BaseRepository<FriendCollection>
    {
        public Task AddFriendAsync(string AccountId, string FriendId);
        public  Task<UpdateResult> RemoveFriendAsync(string AccountId, string FriendId);
        public Task<GetFriendsByAccountConvert?> GetFriendAysnc(string AccountId,int skip, int limit);
        public  Task<GetInfoWaitAccecptConvert?> GetInfoFromWatiList(string AccountId, int skip, int limit);
        public Task AcceptFriend(string myId, string WaitListId);
        public Task AddToWaitlistAsync(string AccountId, string WaitListId);
    }
}
