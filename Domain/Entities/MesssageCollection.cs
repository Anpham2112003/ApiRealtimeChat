using Domain.Entites;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class MesssageCollection
    {
        public ObjectId Id { get; set; }
        public int Page { get; set; }
        public int Count { get; set; }
        public List<Message>? Messages { get; set; }

    }
}
