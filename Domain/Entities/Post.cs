using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Post:Change
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id {  get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string? AccountId {  get; set; }
        public string? Content {  get; set; }
        public int Likes {  get; set; }
        public List<ObjectId>? ListLike { get; set; }
        public List<Comment>? Comments { get; set; }
        public int TotalComment {  get; set; }
        public bool AllowComment {  get; set; }
        public bool HiddenComment {  get; set; }

        public List<string>? Images { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
