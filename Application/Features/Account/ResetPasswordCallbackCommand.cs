using Application.Errors;
using Domain.Entities;
using Domain.Ultils;
using Infrastructure.Services;
using Infrastructure.Services.MailService;
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
                if (string.IsNullOrEmpty(request.token) || !JwtLibrary.TryValidateToken(request.token, _configuration["Jwt:ResetPasswordKey"]! , out var Claims))

                    return Result<string>.Failuer(AccountError.TokenNotValid(request.token!));


                var email = Claims.FindFirstValue(ClaimTypes.Email);

                var newPassword = DateTime.UtcNow.ToString("yyyy’-‘MM’-‘dd’T’HH’:’mm’:’ss.fffffffK");

                var hashPassword = HashLibrary.GenerateHash(newPassword);
               

                await _unitOfWork.accountRepository.UpdatePassword(email, hashPassword);
                
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
