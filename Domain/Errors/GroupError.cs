using Domain.Ultils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Errors
{
    public static class GroupError
    {
        public static Error GroupNotFound => new Error("Group", "Group not exist!");
        public static Error UserNotFound => new Error("Group", "User not exits in Group!");
        public static Error NotPermission => new Error("Group", "You have not permission!");
    }
}
