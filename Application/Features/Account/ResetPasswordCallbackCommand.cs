using Application.Errors;
using Domain.Entities;
using Domain.Ultils;
using Infrastructure.Services;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Account
{
    public class ResetPasswordCallbackCommand:IRequest<Result<string>>
    {
        public string? token {  get; set; }  

    }

    public class HandResetPasswordCallbackCommand : IRequestHandler<ResetPasswordCallbackCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMailerServices _mailerServices;
        private readonly IConfiguration _configuration;

        public HandResetPasswordCallbackCommand(IUnitOfWork unitOfWork, IMailerServices mailerServices, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mailerServices = mailerServices;
            _configuration = configuration;
        }

        public async Task<Result<string>> Handle(ResetPasswordCallbackCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(request.token) 

                || !JwtLibrary.ValidateToken(request.token, _configuration["Jwt:ResetPasswordKey"]!))

                    return Result<string>.Failuer(AccountError.TokenNotValid(request.token!));

                var Claims = JwtLibrary.GetClaimsPrincipalFromToken(request.token, _configuration["Jwt:ResetPasswordKey"]!);

                var email = Claims.FindFirstValue(ClaimTypes.Email);

                var account = await _unitOfWork.accountRepository.FindAccountByEmail(email);

                if (account == null) return Result<string>.Failuer(AccountError.EmailNotFound(email));

                var newPassword = DateTime.UtcNow.ToString();
                var hashPassword = HashLibrary.GenerateHash(newPassword);
                var filter = Builders<AccountCollection>.Filter.Eq(x => x.Email, email);

                var update = Builders<AccountCollection>.Update.Set(x => x.Password, hashPassword);

                await _unitOfWork.accountRepository.UpdateAsync(filter, update);

                var mailConten = new MailContent
                {
                    To = email,
                    Subject = "New Password",
                    Content = $@"<html>
                             <p>
                                 mau khau moi cua ban:
                             </p> 
                                    <br>
                             <p>
                                    {newPassword}
                             </p>
                        </html>"
                };

                await _mailerServices.SendMailAsync(mailConten);

                return Result<string>.Success("success!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            
            
        }
    }
}
