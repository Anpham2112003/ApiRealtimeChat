using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class RoomJoinCollection : BaseCollection
    {
       
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

       
        [BsonRepresentation(BsonType.ObjectId)]
        public string? UserId { get; set; }
        public List<Room>? Rooms { get; set; }
    }
}
