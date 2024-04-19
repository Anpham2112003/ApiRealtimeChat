using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Ultils
{
    public sealed record Error(string Status, string Decripstion)
    {
        public static readonly Error None = new Error(string.Empty, string.Empty);
    }

}
