﻿using Domain.Enums;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ResponeModel
{
    [BsonIgnoreExtraElements]
    public class MembersGroupResponseModel
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? FullName { get; set; }
        public string? Avatar { get; set; }
        public UserState State { get; set; }
        public GroupRoles Role { get; set; }
    }


}
