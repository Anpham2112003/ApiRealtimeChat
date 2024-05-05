using Application.Errors;
using Domain.Settings;
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
                Audience= JwtSetting.Audience,
                Issuer=JwtSetting.Isssser,
                Subject = new ClaimsIdentity(claims),
                Expires = ExpireTime,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256)
            };

            var Tokenhandler = new JwtSecurityTokenHandler();

            var token = Tokenhandler.WriteToken(Tokenhandler.CreateJwtSecurityToken(descriptor)) ;

            return token;
            
        }
        public static bool ValidateToken(string token, string key)
        {
            try
            {
                var prametor = new TokenValidationParameters()
                {
                    ValidateLifetime=true,
                    IssuerSigningKey= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ValidateAudience=true,
                    ValidateIssuer=true,
                    ValidAudience=JwtSetting.Audience,
                    ValidIssuer=JwtSetting.Isssser
                };
                var handler = new JwtSecurityTokenHandler();

                handler.ValidateToken(token, prametor, out SecurityToken security);

                return true;
            }
            catch (Exception)
            {
                throw;
                
            }
        }

        public static Error TryValidateToken(string token, string key, out ClaimsPrincipal? claims)
        {
            try
            {
                var prametor = new TokenValidationParameters()
                {
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidAudience = JwtSetting.Audience,
                    ValidIssuer = JwtSetting.Isssser
                };
                var handler = new JwtSecurityTokenHandler();

                claims =  handler.ValidateToken(token, prametor, out SecurityToken security);

                return Error.None;
            }
            catch (SecurityTokenExpiredException e)
            {
                claims = null;
                return AccountError.TokenExpire(e.Expires);
            }
            catch(Exception ) 
            {
                claims = null;
                return AccountError.TokenNotValid(token);
            }
        }

        public static ClaimsPrincipal GetClaimsPrincipalFromToken(string token , string key)
        {
           
            try
            {
                var prametor = new TokenValidationParameters()
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidAudience = JwtSetting.Audience,
                    ValidIssuer = JwtSetting.Isssser
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
