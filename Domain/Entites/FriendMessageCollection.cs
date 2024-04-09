using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entites
{
    public class FriendMessageCollection : BaseCollection
    {
        public ObjectId Id { get; set; }
        public ObjectId UserCollectionId { get; set; }

        public List<FriendRoomChat>? RoomChat {  get; set; }
    }
}
