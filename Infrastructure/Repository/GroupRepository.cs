using Domain.Entities;
using Domain.Enums;
using Domain.ResponeModel;
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
                    Builders<ConversationCollection>.Filter.Eq("Owners", ObjectId.Parse(MyId)),
                    Builders<ConversationCollection>.Filter.Eq(x => x.IsGroup, true),
                    Builders<ConversationCollection>.Filter.Nin("Owners",ObjIds)
                );

            var update = Builders<ConversationCollection>
                .Update
                .Inc(x=>x.Group!.TotalMember,ObjIds.Count())
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

        public async Task<Member?> FindMemberInGroup(string Id, string MemberId)
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
                .Inc(x=>x.Group!.TotalMember,-1)
                .Pull(x=>x.Owners,ObjectId.Parse(MemberId))
                .PullFilter(x => x.Group!.Members, Builders<Member>.Filter.Eq(x => x.Id, MemberId));

            await _collection!.UpdateOneAsync(filter, update);
        }

       

        public async Task<UpdateResult> LeaveGroup(string Id, string UserId)
        {
            var filter = Builders<ConversationCollection>.Filter
                .And(
                    Builders<ConversationCollection>.Filter.Eq(x => x.Id, Id),
                    Builders<ConversationCollection>.Filter.Eq("Owners", ObjectId.Parse(UserId))
                );

            var update = Builders<ConversationCollection>.Update
                .Pull(x => x.Owners, ObjectId.Parse(UserId))
                .PullFilter(x => x.Group!.Members, Builders<Member>.Filter.Eq(x => x.Id, UserId))
                .Inc(x => x.Group!.TotalMember, -1);
                


            return await _collection!.UpdateOneAsync(filter,update);
        }

        public async Task<IEnumerable<MembersGroupResponseModel>> GetMembersInGroup(string ConversationId, int skip, int limit)
        {
            var aggry = await _collection.Aggregate()
                .Match(x => x.Id == ConversationId && x.IsGroup == true)
                .AppendStage<BsonDocument>(new BsonDocument
                {
                    {
                        "$addFields", new BsonDocument
                        {
                            {
                                "sliceMember", new BsonDocument
                                {
                                    {
                                        "$slice", new BsonArray
                                        {
                                            "$Group.Members",
                                            skip,limit
                                        }
                                    }
                                }
                            }
                        }
                    }
                }).AppendStage<BsonDocument>(new BsonDocument
                {
                    {
                        "$lookup", new BsonDocument
                        {
                            {
                                "from",nameof(UserCollection)
                            },
                            {
                                "localField","sliceMember._id"
                            },
                            {
                                "foreignField","AccountId"
                            },
                            {
                                "pipeline", new BsonArray
                                {
                                    new BsonDocument
                                    {
                                        {
                                            "$project", new BsonDocument
                                            {
                                                {
                                                    "_id",0
                                                },
                                                {
                                                    "AccountId",1
                                                },
                                                {
                                                    "FullName",1
                                                },
                                                {
                                                    "Avatar",1
                                                },
                                                {
                                                    "State",1
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            {
                                "as","Result"
                            }
                        }
                    }
                }).Project(new BsonDocument
                {
                    {
                        "_id",0
                    },
                    {
                        "Members", new BsonDocument
                        {
                            {
                                "$map", new BsonDocument
                                {
                                    {
                                        "input", "$sliceMember"
                                    },
                                    {
                                        "as","item"
                                    },
                                    {
                                        "in", new BsonDocument
                                        {
                                            {
                                                "$mergeObjects", new BsonArray
                                                {
                                                    "$$item",
                                                    new BsonDocument
                                                    {
                                                        {
                                                            "$arrayElemAt", new BsonArray
                                                            {
                                                                "$Result",
                                                                new BsonDocument
                                                                {
                                                                    {
                                                                        "$indexOfArray", new BsonArray
                                                                        {

                                                                                "$Result.AccountId","$$item._id"

                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                })
                .Unwind("Members")
                .ReplaceRoot<BsonDocument>("$Members")
                .As<MembersGroupResponseModel>().ToListAsync();

            return aggry is null ? Enumerable.Empty<MembersGroupResponseModel>() : aggry;
        }

        public async Task<UpdateResult> UpdateRole(string GroupId, string MemberId, GroupRoles role)
        {
            var filter = Builders<ConversationCollection>.Filter.And(
                    Builders<ConversationCollection>.Filter.Eq(x => x.Id, GroupId),
                    Builders<ConversationCollection>.Filter.Eq(x => x.IsGroup, true)
             );

            var update = Builders<ConversationCollection>.Update.Set("Group.Members.$[m].Role", role);

            var arrayFilter = new[]
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument
                {
                    {
                       
                            "m._id",ObjectId.Parse(MemberId)
                        
                    }
                })
            };

            return await _collection!.UpdateOneAsync(filter, update, new UpdateOptions { ArrayFilters = arrayFilter });
        }
    }
}
