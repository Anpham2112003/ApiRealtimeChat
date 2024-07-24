using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ResponeModel
{
    public class ScrollPage<T> where T : class
    {
        public int Index { get; set; }
        public int Limit { get; set; }
        public IEnumerable<T>? Data { get; set; }

        public ScrollPage(int index, int limit, IEnumerable<T>? data)
        {
            Index = index;
            Limit = limit;
            Data = data;
        }

        public ScrollPage()
        {
        }
    }
}
