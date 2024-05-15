using Domain.Entites;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class FriendRoomCollection
    {
        public ObjectId Id { get; set; }
        public ObjectId UserId { get; set; }
        public List<RoomJoin>? RoomJoins { get; set; }

        public FriendRoomCollection(ObjectId id, ObjectId userId)
        {
            Id = id;
            UserId = userId;
            RoomJoins = new List<RoomJoin>();
        }
    }
}
