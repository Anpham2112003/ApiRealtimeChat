using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ResponeModel.BsonConvert
{
    public class GetMessageConvert
    {
        public List<ClientMessageReceiver>? Messages { get; set; }
    }
}
