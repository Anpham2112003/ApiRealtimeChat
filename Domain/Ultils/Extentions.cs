using Domain.Entities;
using Domain.ResponeModel.BsonConvert;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Domain.Ultils
{
    public static class Extentions
    {
        public static string GetIdFromClaim(this ClaimsPrincipal claims)
        {
            var claim = claims.FindFirstValue(ClaimTypes.PrimarySid);

            return claim;
        }

        public static User GetUserFromToken(this ClaimsPrincipal claims)
        {
            var jsonUser = claims.FindFirstValue(ClaimTypes.UserData);

            var User = JsonSerializer.Deserialize<User>(jsonUser);
            
            return User!;
        }
    }
}
