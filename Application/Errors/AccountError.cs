using Application.Ultils;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Errors
{
    public static class AccountError
    {
       
        public static Error AccountAllready(string Email) => new Error("Account.AllReady", $"Account with Email : {Email} was exist!");
        public static Error AccountIncorrect() => new Error("Account.Incorrect", "Account or Password is incorrect!");

    }
}
