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
    public interface IFriendRepository :BaseRepository<FriendCollection>
    {
        public Task AddFriendAsync(string AccountId, string FriendId);
        public  Task<UpdateResult> RemoveFriendAsync(string AccountId, string FriendId);
        public Task<List<UserConvert>> GetFriendAysnc(string AccountId,int skip, int limit);
        public  Task<List<FriendWaitListResponeModel>> GetInfoFromWatiList(string AccountId, int skip, int limit);
        public Task AcceptFriend(string myId, string WaitListId);
        public Task AddToWaitlistAsync(string AccountId, string MyId);
        public Task<UpdateResult> CancelFriendResquestAsync(string MyId, string CancelId);
        public  Task<List<SearchFriendResponeModel>> FriendSearchAsync(string name, string Id);
        public  Task<UpdateResult> RejectFriendRequest(string AccountId, string RejectId);
    }
}
