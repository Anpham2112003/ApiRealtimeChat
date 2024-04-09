using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entites
{
    public class Friend : BaseCollection
    {
        public ObjectId Id { get; set; }
        public string? Name { get; set; }

        public Friend(ObjectId id, string? name)
        {
            Id = id;
            Name = name;
        }
    }
}
