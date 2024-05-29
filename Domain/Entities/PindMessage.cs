using Domain.Enums;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PindMessage
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id {  get; set; }
        public string? AccountId {  get; set; }
        public string? By {  get; set; }
        public string? Content {  get; set; }
        public MessageType Type { get; set; }
    }
}
