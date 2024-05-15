using Domain.Entites;
using Domain.ResponeModel.BsonConvert;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.BaseRepository
{
    public interface IGroupRoomRepository:BaseRepository<GroupRoomCollection>
    {
        public Task AddToGroupRoom(ObjectId UserId, ObjectId RoomId);
        public Task RemoveGroupRoom(ObjectId UserId, ObjectId RoomId);
        public Task<GroupQueryConvert> GetGroupAsync(ObjectId UserId, int skip, int limit);
    }
}
