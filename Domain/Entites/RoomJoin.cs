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

        public RoomJoin(ObjectId userId, ObjectId groupId)
        {
            UserId = userId;
            GroupId = groupId;
        }


    }
}
