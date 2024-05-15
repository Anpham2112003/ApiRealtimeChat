using Domain.Entites;
using Domain.ResponeModel.BsonConvert;
using Infrastructure.MongoDBContext;
using Infrastructure.Repository.BaseRepository;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class GroupRoomRepository : AbstractRepository<GroupRoomCollection>, IGroupRoomRepository
    {
        public GroupRoomRepository(IMongoDB mongoDB) : base(mongoDB)
        {
            base._collection=mongoDB.GetCollection<GroupRoomCollection>(nameof(GroupRoomCollection));
        }

        public async Task AddToGroupRoom(ObjectId UserId,ObjectId RoomId)
        {
            var filter = Builders<GroupRoomCollection>.Filter.Eq(x => x.UserId, UserId);
            var update = Builders<GroupRoomCollection>.Update.AddToSet(x => x.GroupJoins, RoomId);

            await _collection!.UpdateOneAsync(filter, update, new UpdateOptions
            {
                IsUpsert = true
            });
        }

        public async Task RemoveGroupRoom(ObjectId UserId,ObjectId RoomId)
        {
            var filter = Builders<GroupRoomCollection>.Filter.Eq(x=>x.UserId, UserId);

            var update = Builders<GroupRoomCollection>.Update.Pull(x=>x.GroupJoins,RoomId);

           var result=  await _collection!.UpdateOneAsync(filter, update);

            Debug.Write(result);
        }

        public async Task<GroupQueryConvert> GetGroupAsync(ObjectId UserId , int skip, int limit)
        {
            var aggry = await _collection.Aggregate()
                .Match(x => x.UserId == UserId)
                .AppendStage<BsonDocument>(new BsonDocument
                {
                    {
                        "$addFields", new BsonDocument
                        {
                            {
                                "Room", new BsonDocument
                                {
                                    {
                                        "$slice",new BsonArray
                                        {
                                            "$GroupJoins",
                                             skip,
                                            limit
                                        }       
                                    }
                                }
                            }
                        }
                    }
                })
                .AppendStage<BsonDocument>(new BsonDocument
                {
                    {
                        "$lookup", new BsonDocument
                        {
                            {
                                "from",nameof(GroupCollection)
                            },
                            {
                                "localField","Room"
                            },
                            {
                                "foreignField","_id"
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
                                                    "UserId",0
                                                },
                                                {
                                                    "Members",0
                                                },
                                                {
                                                    "Messages",0
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            {
                                "as","Groups"
                            }
                        }
                    }
                }).
                Project(new BsonDocument
                {
                    {
                        "_id",0
                    },
                    {
                        "TotalGroup",new BsonDocument
                        {
                            {
                                "$size","$GroupJoins"
                            }
                        }
                    },
                    {
                        "Groups",1
                    }
                }).As<GroupQueryConvert>().FirstOrDefaultAsync();

            Debug.WriteLine(aggry);

            return aggry;
        }

    }
}
