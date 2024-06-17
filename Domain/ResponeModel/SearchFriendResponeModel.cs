using Domain.Enums;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ResponeModel
{
    public class SearchFriendResponeModel
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? AccountId { get; set; }

        public string? FullName { get; set; }
        public bool Gender { get; set; }
        public string? Avatar { get; set; }
        public UserState State { get; set; }
    }

}
