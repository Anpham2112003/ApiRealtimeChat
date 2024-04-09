using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entites
{
    public class FriendChatMessage : BaseCollection
    {
        public ObjectId Id { get; set; }
        public ObjectId FriendRoomChatId { get; set; }
        public List<Message>? Messages { get; set; }
    }
}
