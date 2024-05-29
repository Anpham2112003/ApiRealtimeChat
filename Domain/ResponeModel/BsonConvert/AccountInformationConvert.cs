using Domain.Entities;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ResponeModel.BsonConvert
{
    public class AccountInformationConvert
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id {  get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public bool IsDelete { get; set; }
        public User? User { get; set; }
    }
}
