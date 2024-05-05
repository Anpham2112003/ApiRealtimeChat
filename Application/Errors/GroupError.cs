using Application.Ultils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Errors
{
    public static class GroupError
    {
        public static Error NotFound => new Error("Group", " Group not Exist!");
    }
}
