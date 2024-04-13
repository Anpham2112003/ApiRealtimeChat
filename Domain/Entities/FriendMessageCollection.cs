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
        public string? AscId { get; set; }
        public bool IsBlock {  get; set; }
        public List<Message>? Messages {  get; set; }
    }
}
