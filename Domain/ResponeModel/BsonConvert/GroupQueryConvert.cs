using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ResponeModel.BsonConvert
{
    public class GroupQueryConvert
    {
        public int TotalGroup {  get; set; }
        public List<GroupConvert>? Groups { get; set; }
    }
}
