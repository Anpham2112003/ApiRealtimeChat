using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entites
{
    internal interface BaseCollection
    {
        ObjectId Id { get; set; }
    }
}
