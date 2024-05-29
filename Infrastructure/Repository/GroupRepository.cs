using Domain.Entities;
using Infrastructure.MongoDBContext;
using Infrastructure.Repository.BaseRepository;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class GroupRepository : AbstractRepository<ConversationCollection>, IGroupRepository
    {
        public GroupRepository(IMongoDB mongoDB) : base(mongoDB)
        {
            _collection = mongoDB.GetCollection<ConversationCollection>(nameof(ConversationCollection));
        }

        public async Task<UpdateResult> UpdateAvatarGroupAsync(string Id,string AvatarUrl)
        {
            var filter =Builders<ConversationCollection>.Filter.Where(x=>x.Id== Id&&x.IsGroup==true);

            var update = Builders<ConversationCollection>.Update
                .Set(x=>x.Group!.UpdatedAt,DateTime.UtcNow)
                .Set(x=>x.Group!.Avatar, AvatarUrl);

           return await _collection!.UpdateOneAsync(filter, update);
        }

        public async Task<UpdateResult> AddManyMemberToGroup(string MyId,string GroupId, IEnumerable<string> Ids)
        {
            var members = new HashSet<Member>();
            var ObjIds=new HashSet<ObjectId>();
            foreach (var item in Ids!)
            {
                var member = new Member(item, Domain.Enums.GroupRoles.Member);

                members.Add(member);
                ObjIds.Add(ObjectId.Parse(item));
            }

            var filter = Builders<ConversationCollection>
                .Filter.And(
                    Builders<ConversationCollection>.Filter.Eq(x => x.Id, GroupId),
                    Builders<ConversationCollection>.Filter.Eq("Onwers", MyId),
                    Builders<ConversationCollection>.Filter.Eq(x => x.IsGroup, true)
                );

            var update = Builders<ConversationCollection>
                .Update
                .AddToSetEach(x=>x.Owners,ObjIds)
                .PushEach(x => x.Group!.Members, members);

           return await _collection!.UpdateOneAsync(filter, update);
        }

        public async Task CreateGroupAsync(ConversationCollection conversation)
        {
            await _collection!.InsertOneAsync(conversation);
        }

        public async Task<UpdateResult> RenameGroupAsync(string Id, string Name)
        {
            var filter = Builders<ConversationCollection>
                .Filter.Where(x=>x.Id==Id&&x.IsGroup == true);

            var update = Builders<ConversationCollection>
                .Update
                .Set(x=>x.Group!.UpdatedAt,DateTime.UtcNow)
                .Set(x=>x.Group!.Name,Name);

            return   await _collection!.UpdateOneAsync(filter,update);

        }

        public async Task<Member?> GetMemberInGroup(string Id, string MemberId)
        {
            var buider = Builders<ConversationCollection>.Filter.Where(x=>x.Id==Id&&x.IsGroup==true);
            var project = Builders<ConversationCollection>.Projection.Expression(x => x.Group!.Members!.FirstOrDefault(x=>x.Id==MemberId));

            var result = await _collection.Find(buider).Project(project)!.FirstOrDefaultAsync();

            return result;
        }

        public async Task KickMemberInGroup(string Id, string MemberId)
        {
            var filter = Builders<ConversationCollection>
                .Filter.Where(x=>x.Id == Id&&x.IsGroup==true);

            var update = Builders<ConversationCollection>
                .Update
                .Pull(x=>x.Owners,ObjectId.Parse(MemberId))
                .PullFilter(x => x.Group!.Members, Builders<Member>.Filter.Eq(x => x.Id, MemberId));

            await _collection!.UpdateOneAsync(filter, update);
        }

        public async Task<List<string>> DeleteGroupAsync(string Id)
        {
            var filter = Builders<ConversationCollection>.Filter.Eq(x=>x.Id, Id);
            var project = Builders<ConversationCollection>.Projection.Expression(x => x.Group!.Members!.Select(x=>x.Id).ToList());

            var result =await _collection!.FindOneAndDeleteAsync(filter,new FindOneAndDeleteOptions<ConversationCollection, List<string>>
            {
                Projection=project!
            });

            return result;
           
        }

        public async Task<UpdateResult> LeaveGroup(string Id, string UserId)
        {
            var filter = Builders<ConversationCollection>.Filter
                .And(
                    Builders<ConversationCollection>.Filter.Eq(x => x.Id, Id),
                    Builders<ConversationCollection>.Filter.ElemMatch(x => x.Messages, UserId)
                );

            var update = Builders<ConversationCollection>.Update
                .Pull(x=>x.Owners,ObjectId.Parse(UserId))
                .PullFilter(x=>x.Group!.Members,Builders<Member>.Filter.Eq(x=>x.Id, UserId));

            return await _collection!.UpdateOneAsync(filter,update);
        }
    }
}
