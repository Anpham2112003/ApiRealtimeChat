using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entites
{
    public class Message : BaseCollection, SoftDelete,Change
    {
        public ObjectId Id { get; set; }
        public ObjectId UserId { get; set; }
        public string? UserName { get; set; }
        public string? Content {  get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDelete { get ; set ; }
        public DateTime DeletedAt { get; set; }
        
    }
}
