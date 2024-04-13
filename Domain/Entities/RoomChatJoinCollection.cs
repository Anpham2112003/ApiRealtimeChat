﻿using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entites
{
    public class RoomChatJoinCollection : BaseCollection
    {
       
        public ObjectId Id { get; set; }
        public ObjectId UserId { get; set; }
        public List<RoomJoin>? RoomJoins { get; set; }

        public RoomChatJoinCollection(ObjectId id, ObjectId userId)
        {
            Id = id;
            UserId = userId;
            RoomJoins = new List<RoomJoin>();
        }
    }
}