using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public interface SoftDelete
    {
        public bool IsDelete { get; set; }

        public DateTime DeletedAt { get; set; }
    }
}
