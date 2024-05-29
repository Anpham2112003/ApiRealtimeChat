
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
        public List<UserConvert>? Result { get; set; }
    };


   
}
