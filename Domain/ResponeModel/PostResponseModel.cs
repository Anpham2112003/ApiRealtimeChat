using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.ResponeModel
{
    public class PostResponseModel
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string? AccountId { get; set; }
        public string? FullName {  get; set; }
        public string? Avatar {  get; set; }
        public UserState State { get; set; }
        public int PageId { get; set; }
        public string? Content { get; set; }
        public int Likes { get; set; }
        public bool Liked { get; set; }
        public List<string>? Images { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
