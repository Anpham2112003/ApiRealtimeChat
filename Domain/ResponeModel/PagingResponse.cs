using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ResponeModel
{
    public class PagingRespone<T>
    {
        public int Index { get; set; }
        public int Limit { get; set; }
        public T? Data { get; set; }

        public PagingRespone(int index, int limit, T? data=default)
        {
            Index = index;
            Limit = limit;
            Data = data;
        }
    }
}
