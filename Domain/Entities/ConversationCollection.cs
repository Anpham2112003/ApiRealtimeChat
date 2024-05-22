using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ConversationCollection : BaseCollection,Change
    {
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public List<string>? Owners { get; set; }
        public List<Message>? Messages { get; set; }
        public bool IsGroup {  get; set; }

        [BsonIgnoreIfNull]
        public Group? Group { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
