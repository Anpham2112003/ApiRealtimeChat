using Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entites
{
    public class UserCollection : BaseCollection
    {
       
        public ObjectId Id { get; set; }
        public ObjectId AccountId { get; set; }
        public string? FistName { get; set; }
        public string? LastName {  get; set; }
        public string? FullName {  get; set; }
        public bool Gender { get; set; } 
        public string? Avatar { get; set; }
        public UserState State { get; set; }
        public DateTime UpdatedAt { get; set; }

       
    }
}
