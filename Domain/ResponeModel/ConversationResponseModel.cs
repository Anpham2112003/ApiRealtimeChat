using Domain.Entities;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ResponeModel
{
    public class ConversationResponseModel
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public List<UserResponseModel>? Owners { get; set; }
        public List<ClientMessageResponseModel>? Messages { get; set; }
        public List<ClientMessageResponseModel>? Pinds { get; set; }
        public bool IsGroup { get; set; }

        [BsonIgnoreIfNull]
        public Group? Group { get; set; }
        public DateTime Seen { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
