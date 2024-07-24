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
       
        public  Task RemoveFriendAsync(string AccountId, string FriendId);
        public Task<List<UserResponseModel>> GetFriendAysnc(string AccountId,int skip, int limit);
        public  Task<List<UserResponseModel>> GetInfoFromWatiList(string AccountId, int skip, int limit);
        public Task<BulkWriteResult> AcceptFriend(string AccountId, string FriendId);
        public Task<UpdateResult> AddToWaitlistAsync(string AccountId, string MyId);
        public Task<UpdateResult> CancelFriendResquestAsync(string MyId, string CancelId);
        public  Task<List<UserResponseModel>> FriendSearchAsync(string name, string Id);
        public  Task<UpdateResult> RejectFriendRequest(string AccountId, string RejectId);
        public  Task<List<UserResponseModel>?> GetFriendNotInGroup(string MyId, string GroupId, int skip, int limit);
        public  Task<bool> HasInvitedOrFriend(string FriendId, string MyId);
        public  Task<bool> HasInInviteList(string MyId, string AccountId);
        public  Task<bool> HasFriend(string AccountId, string FriendId);
    }
}
