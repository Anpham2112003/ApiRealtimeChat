using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Room
    {
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string? ConversationId { get; set; }
        public DateTime LastJoin { get; set; }
    }
}
