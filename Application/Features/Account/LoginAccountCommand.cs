using Application.Errors;
using Domain.ResponeModel;
using Domain.Settings;
using Domain.Ultils;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Account
{
    public class LoginAccountCommand:IRequest<Result<LoginResponseModel>>
    {
        public string? Email { get; set; }
        public string? Password { get; set; }

        public LoginAccountCommand(string? email, string? password)
        {
            Email = email;
            Password = password;
        }
    }


    public class HandLoginAccount : IRequestHandler<LoginAccountCommand, Result<LoginResponseModel>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOptionsMonitor<JwtSetting> _options;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HandLoginAccount(IUnitOfWork unitOfWork, IOptionsMonitor<JwtSetting> options, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _options = options;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<LoginResponseModel>> Handle(LoginAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var checkAccount = await _unitOfWork.accountRepository.FindAccountByEmail(request.Email!);

                if (checkAccount is null || checkAccount.IsDelete) return Result<LoginResponseModel>.Failuer(AccountError.AccountIncorrect());

                var compare = HashLibrary.VerifyHash(checkAccount!.Password!, request!.Password!);

                if(compare is false) return Result<LoginResponseModel>.Failuer(AccountError.AccountIncorrect());


                var claims = new[]
                {
                    new Claim(ClaimTypes.Email, request.Email!),
                    new Claim(ClaimTypes.PrimarySid,checkAccount.Id!.ToString())
                };


                var accessToken = JwtLibrary.GenerateToken(_options.CurrentValue.AccessKey!, claims, DateTime.UtcNow.AddMinutes(1));

                var refreshToken = JwtLibrary.GenerateToken(_options.CurrentValue.ReFreshKey!, claims, DateTime.UtcNow.AddDays(7));

                

                return Result<LoginResponseModel>.Success(new LoginResponseModel(checkAccount.Id.ToString(), accessToken, refreshToken));
            }
            catch (Exception)
            {

                throw;
            }
            
        }
    }
}
