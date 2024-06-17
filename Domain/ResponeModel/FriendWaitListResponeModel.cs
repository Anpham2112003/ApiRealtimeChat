using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.ResponeModel
{

    public class FriendWaitListResponeModel
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string? AccountId { get; set; }
        public string? FullName { get; set; }
        public string? Avatar { get; set; }
        public UserState State { get; set; }
    }
}
