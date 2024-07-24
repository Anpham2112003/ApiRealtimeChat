using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Comment:Change
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id {  get; set; }

        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? AccountId {  get; set; }

        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public  string? ParentId {  get; set; }
        public string? Content {  get; set; }
        public string? ImageUrl{ get; set; }
        public int TotalChildComment {  get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
