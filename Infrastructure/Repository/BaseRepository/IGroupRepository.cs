
using Domain.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.BaseRepository
{
    public  interface IGroupRepository:BaseRepository<ConversationCollection>
    {
        public Task CreateGroupAsync(ConversationCollection conversation);
        public  Task<UpdateResult> AddManyMemberToGroup(string MyId, string GroupId, IEnumerable<string> Ids);
        public Task<UpdateResult> RenameGroupAsync(string Id , string Name);
        public  Task<UpdateResult> UpdateAvatarGroupAsync(string Id, string AvatarUrl);
        public Task<Member?> GetMemberInGroup(string Id, string MemberId);
        public Task KickMemberInGroup(string Id, string MemberId);
        public Task<List<string>> DeleteGroupAsync(string Id);
        public Task<UpdateResult> LeaveGroup(string Id, string UserId);



    }
}
