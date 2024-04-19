using Domain.Enums;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entites
{
    public class AccountCollection : BaseCollection, SoftDelete
    {
        public ObjectId Id { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public AccountType AccountType { get; set; }
        public AccountSate AccountSate { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDelete { get; set; }
        public DateTime DeletedAt { get; set; }

    }
}
