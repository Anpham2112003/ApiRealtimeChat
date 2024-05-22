using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Friend
    {
       
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public DateTime createdAt = DateTime.Now;

        public Friend(string id)
        {
            Id = id;

        }
    }
}
