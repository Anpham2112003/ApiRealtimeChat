using Domain.Ultils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Errors
{
    public static class RoomError
    {
        public static Error NotFound => new Error("Room", "Not found room!");
    }
}
