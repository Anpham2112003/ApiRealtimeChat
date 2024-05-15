using Domain.Entities;
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
        public List<MesssageCollection>? Messages {  get; set; }

        public FriendMessageCollection(ObjectId id, Message message)
        {
            Id = id;
           
            Messages = new List<MesssageCollection>()
            {
                new MesssageCollection() {
                    Id=ObjectId.GenerateNewId(),
                    Page=0,
                    Count=1,
                    Messages=new List<Message>
                    {
                        message
                    }
                }
            };
        }
    }
}
