using Domain.Ultils;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Errors
{
    public static class UserError
    {
        public static Error UserNotFound(string id) => new Error("User.NotFound", $"User with AccountId :{id} was not exist!");
        public static Error AvatarNull => new Error("User.Avatar", "Avatar  is null!");
    }
}
