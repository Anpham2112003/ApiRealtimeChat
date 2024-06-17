using Domain.Enums;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    [BsonIgnoreExtraElements]
    public class User
    {
        [BsonElement("FullName")]
        public string? Name { get; set; }

        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]       
        public string? AccountId {  get; set; }
        public string? Avatar {  get; set; }
        public UserState State { get; set; }
    }
}
