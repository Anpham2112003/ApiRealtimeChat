using Domain.Ultils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Errors
{
    public static class ConversationError
    {
        public static Error NotFound => new Error("Conversation", "NotFound");
    }
}
