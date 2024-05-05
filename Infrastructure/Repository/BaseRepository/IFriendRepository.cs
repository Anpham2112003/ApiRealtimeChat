using Domain.Entites;
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
        public Task AddFriendAsync(ObjectId AccountId, ObjectId FriendId);
        public  Task<UpdateResult> RemoveFriendAsync(ObjectId AccountId, ObjectId FriendId);
        public Task<GetFriendsByAccountConvert?> GetFriendAysnc(ObjectId AccountId,int skip, int limit);
        public  Task<GetInfoWaitAccecptConvert?> GetInfoFromWatiList(ObjectId AccountId, int skip, int limit);
        public Task AcceptFriend(ObjectId myId, ObjectId WaitListId);
        public Task AddToWaitlistAsync(ObjectId AccountId, ObjectId WaitListId);
    }
}
