﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ConversationCollection : BaseCollection
    {
        public ConversationCollection()
        {
            
        }

        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public List<ObjectId>? Owners { get; set; }
        public List<Message>? Messages { get; set; }
        public List<ObjectId>? MessagePinds { get; set; }
        public bool IsGroup {  get; set; }

        [BsonIgnoreIfNull]
        public Group? Group { get; set; }
        
        public DateTime Seen { get; set; }
        public DateTime CreatedAt { get; set; }
        
    }
}
