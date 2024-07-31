using Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ResponeModel
{
    public class NotificationResponseModel
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string? AccountId { get; set; }
        public string? FullName { get; set; }
        public string? Avatar { get; set; }
        public UserState State { get; set; }
        public string? Content { get; set; }
        public NotificationType Type { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
