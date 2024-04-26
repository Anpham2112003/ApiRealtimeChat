using Application.Errors;
using Application.Ultils;
using Infrastructure.Services;
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
        private readonly IMailService _mailService;
        private readonly IConfiguration _configuration;

        public HandResetpasswordCommand(IUnitOfWork unitOfWork, IMailService mailService, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mailService = mailService;
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
                    new Claim(ClaimTypes.PrimarySid,checkEmail.Id.ToString())
                };


                var token = JwtLibrary.GenerateToken(_configuration["Jwt:RestPasswordKey"]!,claims, DateTime.UtcNow.AddMinutes(5));

                var urlRiderect = new UriBuilder();

                urlRiderect.Scheme = _configuration["ResetPasswordUrl:Schema"];
                urlRiderect.Host = _configuration["ResetPasswordUrl:Host"];
                urlRiderect.Path = _configuration["ResetPasswordUrl:Path"];
                urlRiderect.Port = int.Parse(_configuration["ResetPasswordUrl:Port"]!);


                var Mail = new MailContent
                {
                    To = request.Email!,
                    Subject = "Reset Password",
                    Content = ""
                };
                return Result<string>.Success(urlRiderect.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
           
        }
    }
}
