using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ResponeModel.BsonConvert
{
    public class ResultQueyMessage
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id {  get; set; }
        public List<MessagesQueryConvert>? Messages { get; set; }
    }
}
