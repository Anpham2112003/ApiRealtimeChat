using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ResponeModel
{
    public class Event
    {
        public EventType EventType { get; set; }
        public string? EventMessage {  get; set; }
    }
}
