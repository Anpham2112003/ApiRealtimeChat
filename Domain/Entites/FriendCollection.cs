using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entites
{
    public class FriendCollection : BaseCollection
    {
        public ObjectId Id { get; set; }
        public ObjectId UserCollectionId {  get; set; }
        public List<Friend>? Friends { get; set; }

    }
}
