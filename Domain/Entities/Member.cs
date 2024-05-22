using Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Member
    {

       
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public GroupRoles Role { get; set; }

        public Member(string id, GroupRoles role)
        {
            Id = id;

            Role = role;
        }
    }
}
