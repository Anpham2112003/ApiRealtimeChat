using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Group
    {
        public string? Name {  get; set; }
        public string? Avatar {  get; set; }
        public int TotalMember {  get; set; }
        public List<Member>? Members { get; set; }
        public List<MessagePind>? MessagePinds { get; set; }
    }
}
