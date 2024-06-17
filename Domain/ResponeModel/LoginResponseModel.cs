using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ResponeModel
{
    public class LoginResponseModel
    {
        public string? AccountId {  get; set; }
        public string? AccessToken {  get; set; }
        public string? ReFreshToken {  get; set; }

        public LoginResponseModel(string? accountId, string? accessToken, string? reFreshToken)
        {
            AccountId = accountId;
            AccessToken = accessToken;
            ReFreshToken = reFreshToken;
        }

        public LoginResponseModel()
        {
        }
    }
}
