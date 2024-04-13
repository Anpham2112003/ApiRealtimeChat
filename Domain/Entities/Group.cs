using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entites
{
    public class Group
    {
        public ObjectId Id { get; set; }
        public string? Name { get; set; }
        public List<Member>? Members { get; set; }
        public List<Message>? Messages { get; set; }

        public Group(ObjectId id, string? name)
        {
            Id = id;
            Name = name;
            Members = new List<Member>();
            Messages = new List<Message>();
        }
    }
}
