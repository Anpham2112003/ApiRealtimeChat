using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entites
{
    public class Friend 
    {
        public ObjectId Id { get; set; }
        public DateTime createdAt =DateTime.Now;

        public Friend(ObjectId id)
        {
            Id = id;
           
        }
    }
}
