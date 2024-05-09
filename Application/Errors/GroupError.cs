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
        public static Error NotPermission => new Error("Group", "You not have permission!");
        public static Error MemberExist => new Error("Group", "Member exist in group!");
        public static Error MemberNotFound => new Error("Group", "Member not found");
    }
}
