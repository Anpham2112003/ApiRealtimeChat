using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Ultils
{
    public static class Extentions
    {
        public static ObjectId GetIdFromClaim(this ClaimsPrincipal claims)
        {
            var claim = claims.FindFirstValue(ClaimTypes.PrimarySid);

            return ObjectId.Parse(claim);
        }
    }
}
