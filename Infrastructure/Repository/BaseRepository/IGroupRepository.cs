using Domain.Entites;
using Domain.Enums;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.BaseRepository
{
    public interface IGroupRepository:BaseRepository<GroupCollection>
    {
        public Task<ObjectId> CreateGroup(ObjectId UserId,string GroupName);
        public Task AddMemberToGroup(ObjectId GroupId, ObjectId UserId, Member member);
        public Task<Member?> CheckMemberInGroupAsync(ObjectId GroupId, ObjectId UserId);
        public Task RenameGroup(ObjectId GroupId, string GroupName);
        public Task UpdateAvatarGroupAsync(ObjectId GroupId,string AvatarUrl);
        public Task<GroupCollection?> GetAvatarGroupAsync(ObjectId Groupid);
        public Task UpdateRoleInGroup(ObjectId GroupId,ObjectId Id, GroupRoles roles);
        public Task RemoveMemberInGroup(ObjectId GroupId, ObjectId MemberId);
        public Task<List<Member>> GetMembersInGroupAsync(ObjectId GroupId, int skip, int limmit);
        public Task RemoveGroup(ObjectId GroupId);

    }
}
