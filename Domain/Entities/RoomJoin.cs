using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entites
{
    public class RoomJoin
    {

        public ObjectId UserId { get; set; }
        public ObjectId GroupId { get; set; }
        public bool IsGroup {  get; set; }
        public string? GroupName {  get; set; }
        public DateTime LasttJoin { get; set; }
        public RoomJoin(ObjectId userId, ObjectId groupId, bool isGroup, string? groupName)
        {
            UserId = userId;
            GroupId = groupId;
            IsGroup = isGroup;
            GroupName = groupName;
        }
    }
}
