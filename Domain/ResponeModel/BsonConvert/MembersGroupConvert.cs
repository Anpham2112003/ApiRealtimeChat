using Domain.Enums;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ResponeModel.BsonConvert
{
    [BsonIgnoreExtraElements]
    public class MembersGroupConvert
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id {  get; set; }

        [BsonElement("FullName")]
        public string? Name { get; set; }
        public string? Avatar {  get; set; }
        public UserState State { get; set; }
        public GroupRoles Role { get; set; }
    }

    public class ListMemberInGroupConvert
    {
        public List<MembersGroupConvert>? Members { get; set; }
    }
}
