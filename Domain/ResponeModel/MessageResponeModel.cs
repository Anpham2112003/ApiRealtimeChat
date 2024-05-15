using Domain.Entites;
using Domain.ResponeModel.BsonConvert;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ResponeModel
{
    public class MessageResponeModel
    {
        public int Page { get; set; }
        public int Skip {  get; set; }
        public int Limit {  get; set; }
        public IEnumerable<MessagesQueryConvert>? Data { get; set; }
    }
}
