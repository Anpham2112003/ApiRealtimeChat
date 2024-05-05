using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ResponeModel.BsonConvert
{
    public class GetInfoWaitAccecptConvert
    {
        public List<FriendResultWaitList>? waitlist {  get; set; }
    }

    public class FriendResultWaitList
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string? AccountId { get; set; }
        public string? FullName { get; set; }
        public string? Avatar { get; set; }
    }
}
