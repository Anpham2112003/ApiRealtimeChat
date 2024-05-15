using Application.Ultils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Errors
{
    public static class MessageError
    {
        public static Error NotFound => new Error("Message", "Message is Empty!");
        public static Error ColectionNotFound => new Error("Message", "Collection Not Found");
        public static Error NotPermission => new Error("Message", "You have not permission delete!");
    }
}
