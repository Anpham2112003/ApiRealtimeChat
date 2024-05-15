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
    public class MessagesQueryConvert
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id {  get; set; }

        public MessageType MessageType { get; set; }
        public string? Content {  get; set; }
        public bool IsDelete { get; set; }
        public UserConvert? User { get; set; }

    }
}
