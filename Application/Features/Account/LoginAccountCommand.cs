using Application.Errors;
using Domain.ResponeModel;
using Domain.Settings;
using Domain.Ultils;
using Infrastructure.Services.RedisSevices;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
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
        private readonly IRedisService _redisService;
        public HandLoginAccount(IUnitOfWork unitOfWork, IOptionsMonitor<JwtSetting> options, IHttpContextAccessor httpContextAccessor, IRedisService redisService)
        {
            _unitOfWork = unitOfWork;
            _options = options;
            _httpContextAccessor = httpContextAccessor;
            _redisService = redisService;
        }

        public async Task<Result<LoginResponseModel>> Handle(LoginAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var checkAccount = await _unitOfWork.accountRepository.GetAccountInformationAsync(request.Email!);
                
                if (checkAccount is null || checkAccount.IsDelete) 
                    return Result<LoginResponseModel>.Failuer(AccountError.AccountIncorrect());

                var compare = HashLibrary.VerifyHash(checkAccount!.Password!, request!.Password!);

                var jsonUser = JsonSerializer.Serialize(checkAccount.User);
                
                if(compare is false) return Result<LoginResponseModel>.Failuer(AccountError.AccountIncorrect());


                var claims = new[]
                {
                    new Claim(ClaimTypes.Email, request.Email!),
                    new Claim(ClaimTypes.PrimarySid,checkAccount.Id!.ToString()),
                    new Claim(ClaimTypes.UserData,jsonUser),
                    
                };

                

                var accessToken = JwtLibrary.GenerateToken(_options.CurrentValue.AccessKey!, claims, DateTime.UtcNow.AddMinutes(1));

                var refreshToken = JwtLibrary.GenerateToken(_options.CurrentValue.ReFreshKey!, claims, DateTime.UtcNow.AddDays(7));

                var hashs = new HashEntry[]
                {
                    new HashEntry("Email",checkAccount.Email),
                    new HashEntry("Name",checkAccount.User!.Name),
                    new HashEntry("Avatar",checkAccount.User.Avatar??""),
                    new HashEntry("State",checkAccount.User.State.ToString()),
                    new HashEntry("ReFreshToken",refreshToken)
                };

                await _redisService.SetHashValueToRedis(checkAccount.Id, hashs);

                await _unitOfWork.accountRepository.UpdateTokenUser(checkAccount.Id, refreshToken);

                return Result<LoginResponseModel>.Success(new LoginResponseModel(checkAccount.Id.ToString(), accessToken, refreshToken));
            }
            catch (Exception)
            {

                throw;
            }
            
        }
    }
}
