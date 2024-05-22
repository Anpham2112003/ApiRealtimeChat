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
    public class AccountCollection : BaseCollection, SoftDelete
    {
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public AccountType AccountType { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDelete { get; set; }
        public DateTime DeletedAt { get; set; }

    }
}
