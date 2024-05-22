using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class FriendCollection : BaseCollection
    {
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

       
        [BsonRepresentation(BsonType.ObjectId)]
        public string? AccountId { get; set; }
        public List<ObjectId> WaitingList { get; set; }
        public List<Friend>? Friends { get; set; }

        public FriendCollection(string id, string accountId)
        {
            Id = id;
            AccountId = accountId;
            WaitingList = new List<ObjectId>();
            Friends = new List<Friend>();
        }
    }


    public class FriendResult
    {



        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string? AccountId { get; set; }

        public ArrayList? Friends { get; set; }
    }
}
