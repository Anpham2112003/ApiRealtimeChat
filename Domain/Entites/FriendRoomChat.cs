﻿using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entites
{
    public class FriendRoomChat : BaseCollection
    {
        public ObjectId Id { get; set; }
        public ObjectId UserId { get; set; }
        public ObjectId FriendId { get; set; }
    }
}
