using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ResponeModel
{
    public class NumberPage<T> where T : class
    {
        public int Page { get; set; }
        public int TotalPage {  get; set; }
        public IEnumerable<T>? Data { get; set; }    
    }
}
