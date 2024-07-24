
using Domain.Entities;
using Domain.ResponeModel;
using Domain.Settings;
using Domain.Ultils;
using Infrastructure.Services.RedisSevices;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Features.Account
{
    public class GoogleLoginCommand:IRequest<Result<LoginResponseModel>>
    {
        public ObjectId Id = ObjectId.GenerateNewId();
        public string? FistName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public DateTime Created => DateTime.UtcNow;
        public bool IsDelete => false;
    }

    public class HandGoogleLoginCommnand : IRequestHandler<GoogleLoginCommand, Result<LoginResponseModel>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOptionsMonitor<JwtSetting> _options;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IRedisService _redisService;
        public HandGoogleLoginCommnand(IUnitOfWork unitOfWork, IOptionsMonitor<JwtSetting> options, IHttpContextAccessor contextAccessor, IRedisService redisService)
        {
            _unitOfWork = unitOfWork;
            _options = options;
            _contextAccessor = contextAccessor;
            _redisService = redisService;
        }

        public async Task<Result<LoginResponseModel>> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var check = await _unitOfWork.accountRepository.GetAccountInformationAsync("Google:" + request.Email!);
                if (check is null)
                {
                    var account = new AccountCollection()
                    {
                        Id = request.Id.ToString(),
                        Email = "Google:" + request.Email,
                        AccountType = Domain.Enums.AccountType.Google,
                        CreatedAt = DateTime.UtcNow,
                        IsDelete = false,
                    };

                    var user = new UserCollection()
                    {

                        Id = ObjectId.GenerateNewId().ToString(),
                        AccountId = request.Id.ToString(),
                        FistName = request.FistName,
                        LastName = request.LastName,
                        FullName = request.FistName + " " + request.LastName,
                        State=Domain.Enums.UserState.Offline,
                        Avatar="",
                        Gender=true,
          
                    };

                    var addAccount = _unitOfWork.accountRepository.InsertAsync(account);

                    var addUser = _unitOfWork.userRepository.InsertUserAsync(user);

                    await Task.WhenAll(addAccount, addUser);

                    var jsonUser = JsonSerializer.Serialize(new UserResponseModel
                    {
                        AccountId = account.Id,
                        Avatar = "",
                        FullName = user.FullName,
                        State = user.State
                    });

                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Email,account.Email),
                        new Claim(ClaimTypes.PrimarySid,account.Id.ToString()),
                        new Claim(ClaimTypes.UserData,jsonUser)
                    };

                    var accessToken = JwtLibrary.GenerateToken(_options.CurrentValue.AccessKey!, claims, DateTime.UtcNow.AddMinutes(1));
                    var refreshToken = JwtLibrary.GenerateToken(_options.CurrentValue.ReFreshKey!, claims, DateTime.UtcNow.AddDays(7));

                    var hashs = new HashEntry[]
                    {
                        new HashEntry("Email",account.Email),
                        new HashEntry("FullName",user.FullName),
                        new HashEntry("Avatar",user.Avatar??""),
                        new HashEntry("State",user.State.ToString()),
                        new HashEntry("ReFreshToken",refreshToken)
                    };

                    await _unitOfWork.accountRepository.UpdateTokenUser(account.Id, refreshToken);

                    await _redisService.SetHashValueToRedis(account.Id, hashs);

                    return Result<LoginResponseModel>.Success(new LoginResponseModel(account.Id.ToString(), accessToken, refreshToken));
                }
                else
                {
                    var jsonUser = JsonSerializer.Serialize(check.User);

                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Email,check.Email!),
                        new Claim(ClaimTypes.PrimarySid,check.Id!.ToString()),
                        new Claim(ClaimTypes.UserData,jsonUser)
                    };

                    var accessToken = JwtLibrary.GenerateToken(_options.CurrentValue.AccessKey!, claims, DateTime.UtcNow.AddMinutes(1));
                    var refreshToken = JwtLibrary.GenerateToken(_options.CurrentValue.ReFreshKey!, claims, DateTime.UtcNow.AddDays(7));

                    var hashs = new HashEntry[]
                   {
                        new HashEntry("Email",check.Email),
                        new HashEntry("FullName",check.User!.FullName),
                        new HashEntry("Avatar",check.User.Avatar??""),
                        new HashEntry("State",check.User.State.ToString()),
                        new HashEntry("ReFreshToken",refreshToken)
                   };

                    await _unitOfWork.accountRepository.UpdateTokenUser(check.Id, refreshToken);
                   
                    await _redisService.SetHashValueToRedis(check.Id,hashs);

                    return Result<LoginResponseModel>.Success(new LoginResponseModel(check.Id.ToString(), accessToken, refreshToken));
                }
            }
            catch (Exception)
            {

                throw;
            }
            
        }
    }
}
