using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entites
{
    public class GroupCollection : BaseCollection
    {
        public ObjectId Id { get; set; }

        public ObjectId UserId { get; set; }
        public List<Group>?  Groups { get; set; }
    }
}
