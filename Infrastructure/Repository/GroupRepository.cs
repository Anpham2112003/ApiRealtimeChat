using Domain.Entites;
using Infrastructure.MongoDBContext;
using Infrastructure.Repository.BaseRepository;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task AddMemberToGroup(ObjectId GroupId, ObjectId UserId)
        {
            var Member = new Member(UserId, "default", Domain.Enums.GroupRoles.Member);

            var filter = Builders<GroupCollection>
                .Filter.Eq(x=>x.Id, GroupId);

            var update = Builders<GroupCollection>
                .Update.AddToSet(x => x.Members, Member);

            await base.UpdateAsync(filter, update);
        }

        public async Task CreateGroup(ObjectId UserId, string GroupName)
        {
            var Group = new GroupCollection(UserId,GroupName);

            await base.InsertAsync(Group);
        }
    }
}
