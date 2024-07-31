using Application.Errors;
using Domain.Ultils;
using Infrastructure.Services;
using Infrastructure.Services.MailService;
using Infrastructure.Unit0fWork;
using MailKit;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Account
{
    public class ResetPasswordCommand:IRequest<Result<string>>
    {
        public string? Email {  get; set; }

    }
    public class HandResetpasswordCommand : IRequestHandler<ResetPasswordCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMailerServices _mailerServices;
        private readonly IConfiguration _configuration;

        public HandResetpasswordCommand(IUnitOfWork unitOfWork, IMailerServices mailerServices, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mailerServices = mailerServices;
            _configuration = configuration;
        }

        public async Task<Result<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var checkEmail = await _unitOfWork.accountRepository.FindAccountByEmail(request.Email!);

                if (checkEmail is null) return Result<string>.Failuer(AccountError.EmailNotFound(request.Email!));

                var claims = new Claim[]
                {
                    new Claim(ClaimTypes.Email, request.Email!),
                    new Claim(ClaimTypes.PrimarySid,checkEmail.Id!.ToString())
                };


                var token = JwtLibrary.GenerateToken(_configuration["Jwt:ResetPasswordKey"]!,claims, DateTime.UtcNow.AddMinutes(5));

                var urlRiderect = new UriBuilder(_configuration["ResetPasswordUrl:BaseUrl"]!);
                urlRiderect.Query = $"token={token}";




                var Mail = new MailContent
                {
                    To = request.Email!,

                    Subject = "Reset Password",

                    Content = @$"<html>
                                
                                <a href=`{ urlRiderect}`>Day la link thay doi mat khau</a>
                                </html>"
                };

                await _mailerServices.SendMailAsync(Mail);

                return Result<string>.Success();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
           
        }
    }
}
