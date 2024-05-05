using Domain.Entities;
using Domain.Enums;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entites
{
    public class GroupCollection:BaseCollection
    {
        public ObjectId Id { get; set; }
        public ObjectId UserId { get; set; }
        public string? Name { get; set; }
        public string? Avatar { get; set; }
        public List<Member>? Members { get; set; }
        public List<MesssageCollection>? Messages { get; set; }

        public GroupCollection(ObjectId Userid, string? name)
        {
            Id = ObjectId.GenerateNewId();
            UserId = Userid;
            Name = name;
            Members = new List<Member>() { new Member(Userid,"Admin",GroupRoles.Admin)};
            Messages = new List<MesssageCollection>();
        }
    }
}
