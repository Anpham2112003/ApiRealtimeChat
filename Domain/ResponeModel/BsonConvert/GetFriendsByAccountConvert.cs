using Domain.Entites;
using Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ResponeModel.BsonConvert
{
    public class GetFriendsByAccountConvert
    {
        public List<FriendResultConvert>? Result { get; set; }
    };


   public class FriendResultConvert
    {
       
        [BsonRepresentation(BsonType.ObjectId)]
        public string? AccountId { get; set; }
        public string? FullName { get; set; }
        public string? Avatar {  get; set; }
        public bool Gender { get; set; }    
        public UserState State { get; set; }
    }
}
