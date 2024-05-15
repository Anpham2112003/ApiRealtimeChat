using Domain.Entites;
using Domain.Enums;
using Domain.ResponeModel.BsonConvert;
using Infrastructure.MongoDBContext;
using Infrastructure.Repository.BaseRepository;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class GroupRepository : AbstractRepository<GroupCollection>, IGroupRepository
    {
        
        public GroupRepository(IMongoDB mongoDB) : base(mongoDB)
        {
            base._collection=mongoDB.GetCollection<GroupCollection>(nameof(GroupCollection));
        }

        public async Task AddMemberToGroup(ObjectId GroupId, ObjectId UserId, Member member)
        {
            var Member = new Member(UserId, "default", Domain.Enums.GroupRoles.Member);

            var filter = Builders<GroupCollection>
                .Filter.Eq(x => x.Id , GroupId);

            var update = Builders<GroupCollection>
                .Update.AddToSet(x=>x.Members,member);

            await base.UpdateAsync(filter, update);
        }

        public async Task<ObjectId> CreateGroup(ObjectId UserId, string GroupName)
        {
            var Group = new GroupCollection(UserId,GroupName);

            await base.InsertAsync(Group);

            return Group.Id;
        }

        public async Task<Member?> CheckMemberInGroupAsync(ObjectId GroupId, ObjectId UserId)
        {
            var filter = Builders<GroupCollection>.Filter.Where(x => x.Id == GroupId && x.Members!.Any(x => x.Id == UserId));

            var project = Builders<GroupCollection>.Projection.Include(x => x.Members);

            var result = await _collection.Find(filter).Project(project).As<GroupCollection>().FirstOrDefaultAsync();
            
            return result.Members?.FirstOrDefault() ;

        }

        public async Task RenameGroup(ObjectId GroupId, string GroupName)
        {
            var filter = Builders<GroupCollection>.Filter.Eq(x => x.Id, GroupId);

            var update = Builders<GroupCollection>.Update.Set(x => x.Name, GroupName);

            await _collection!.UpdateOneAsync(filter, update);
        }

        public async Task UpdateAvatarGroupAsync(ObjectId GroupId, string AvatarUrl)
        {
            var filter = Builders<GroupCollection>.Filter.Eq(x => x.Id, GroupId);

            var update = Builders<GroupCollection>.Update.Set(x => x.Avatar, AvatarUrl);

            await _collection!.UpdateOneAsync(filter,update);
        }

        public async Task<GroupCollection?> GetAvatarGroupAsync(ObjectId Groupid)
        {
            var filter = Builders<GroupCollection>.Filter.Eq(x=>x.Id, Groupid);
            var project = Builders<GroupCollection>.Projection
                .Include(x => x.Avatar);

            var result =  await _collection.Find(filter).Project<GroupCollection>(project).FirstOrDefaultAsync();

            return result;
        }

        public async Task UpdateRoleInGroup(ObjectId GroupId, ObjectId Id, GroupRoles roles)
        {
            var filter = Builders<GroupCollection>.Filter.Where(x=>x.Id == GroupId&& x.Members!.Any(x=>x.Id==Id));

            var update = Builders<GroupCollection>.Update.Set(x => x.Members.FirstMatchingElement().Role, roles);

            await _collection!.UpdateOneAsync(filter, update);
        }

        public async Task RemoveMemberInGroup(ObjectId GroupId, ObjectId MemberId)
        {
            var filter = Builders<GroupCollection>.Filter.Where(x=>x.Id==GroupId&& x.Members!.Any(x=> x.Id==MemberId));

            var update = Builders<GroupCollection>.Update.PopFirst(x => x.Members);

            await _collection!.UpdateOneAsync(filter, update);
        }

        public async Task<List<Member>> GetMembersInGroupAsync(ObjectId GroupId, int skip, int limmit)
        {
            var filter = Builders<GroupCollection>
                .Filter.Eq(x=>x.Id,GroupId);

            var project = Builders<GroupCollection>

                .Projection
                .Exclude(x=>x.Id)
                .Exclude(x=>x.Messages)
                .Exclude(x=>x.Name)
                .Exclude(x=>x.Avatar)
                .Slice(x => x.Members, skip, limmit);
       
                
                
            var result =await _collection.Find(filter)
                .Project<GroupCollection>(project)
                .FirstOrDefaultAsync();

            if( result is null) return new List<Member>();

            return result.Members!;
        }

        public async Task RemoveGroup(ObjectId GroupId)
        {
            var filter = Builders<GroupCollection>.Filter.Eq(x=>x.Id,GroupId);

            await _collection.FindOneAndDeleteAsync(filter);
        }
    }
}
