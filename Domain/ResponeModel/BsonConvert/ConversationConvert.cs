using Domain.Entities;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ResponeModel.BsonConvert
{
    public class ConversationConvert
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public List<User>? Owners { get; set; }
        public List<ClientMessageReceiver>? Messages { get; set; }
        public List<ClientMessageReceiver>? MessagePinds { get; set; }
        public bool IsGroup { get; set; }

        [BsonIgnoreIfNull]
        public Group? Group { get; set; }
        public DateTime Seen { get; set; }
        public DateTime CreatedAt { get; set; }
        
    }
}
