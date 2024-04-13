using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Ultils
{
    public static class JwtLibrary
    {
        public static string GenerateToken(string key,IEnumerable<Claim> claims ,DateTime ExpireTime)
        {
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = ExpireTime,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256)
            };

            var Tokenhandler = new JwtSecurityTokenHandler();

            var token = Tokenhandler.WriteToken(Tokenhandler.CreateJwtSecurityToken(descriptor)) ;

            return token;
            
        }


        public static ClaimsPrincipal ValidateToken(string token , string key)
        {
           
            try
            {
                var prametor = new TokenValidationParameters()
                {
                    ValidateLifetime = true,
                    RequireSignedTokens = true,

                };
                var handler = new JwtSecurityTokenHandler();

                var result = handler.ValidateToken(token, prametor, out SecurityToken security);

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
           
          
        }
    }
}
