using Application.Errors;
using Domain.Entities;
using Domain.Enums;
using Domain.ResponeModel;
using Domain.Settings;
using Domain.Ultils;
using Infrastructure.Services.RedisSevices;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
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
    public class RefreshTokenCommmand:IRequest<Result<LoginResponseModel>>
    {
        public string? Token {  get; set; }
    }

    public class HandRefreshTokenCommand : IRequestHandler<RefreshTokenCommmand, Result<LoginResponseModel>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOptionsMonitor<JwtSetting> _options;
        private readonly IRedisService _redisService;
        private readonly IConfiguration _configuration;
        public HandRefreshTokenCommand(IUnitOfWork unitOfWork, IOptionsMonitor<JwtSetting> options, IRedisService redisService, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _options = options;
            _redisService = redisService;
            _configuration = configuration;
        }

        public async Task<Result<LoginResponseModel>> Handle(RefreshTokenCommmand request, CancellationToken cancellationToken)
        {
            try
            {
                

                var resultValidate = JwtLibrary.TryValidateToken(request.Token!,_options.CurrentValue.ReFreshKey!, out var claims);

                if (resultValidate is false) return Result<LoginResponseModel>.Failuer(new Error("Token","Token not valid"));

                var userId = claims!.GetIdFromClaim();

                var Cache = await _redisService.GetHashValues(userId, new RedisValue[] { "Email","Name", "Avatar", "State","ReFreshToken" });

               
                

                if(Cache is not null && Cache.Any())
                {
                   
                    if (!Cache[4].ToString().Equals(request.Token)) return Result<LoginResponseModel>.Failuer(new Error("Unauthorization", ""));

                    var jsonUser = JsonSerializer.Serialize(new UserResponseModel
                    {
                        AccountId = userId,
                        FullName = Cache[1].ToString(),
                        Avatar = Cache[2].ToString(),
                        State = (UserState)Enum.Parse(typeof(UserState), Cache[3]!.ToString())
                    });

                    var newClaims = new Claim[]
                    {
                        new Claim(ClaimTypes.Email,Cache[0].ToString()!),
                        new Claim(ClaimTypes.PrimarySid,userId),
                        new Claim(ClaimTypes.UserData,jsonUser),
                        new Claim(ClaimTypes.NameIdentifier,userId)
                    };

                    var accesstoken = JwtLibrary.GenerateToken(_options.CurrentValue.AccessKey!,newClaims,DateTime.UtcNow.AddMinutes(1));

                    var refreshtoken = JwtLibrary.GenerateToken(_options.CurrentValue.ReFreshKey!, newClaims, DateTime.UtcNow.AddDays(7));

                    await _redisService.SetHashValueToRedis(userId, new HashEntry[] { new HashEntry("ReFreshToken", refreshtoken) });

                    return Result<LoginResponseModel>.Success(new LoginResponseModel
                    {
                        AccountId = userId,
                        AccessToken = accesstoken,
                        ReFreshToken = refreshtoken,
                    });
                }
                else
                {
                    var email = claims.FindFirstValue(ClaimTypes.Email);

                    var user = await _unitOfWork.accountRepository.GetAccountInformationAsync(email);   

                    var jsonUser = JsonSerializer.Serialize(user!.User);

                    var newClaims = new[]
                    {
                        new Claim(ClaimTypes.Email,email),
                        new Claim(ClaimTypes.PrimarySid,userId),
                        new Claim(ClaimTypes.UserData,jsonUser),
                        new Claim(ClaimTypes.NameIdentifier,userId),
                    };

                   
                     
                    var accesstoken= JwtLibrary.GenerateToken(_options.CurrentValue.AccessKey!,newClaims,DateTime.UtcNow.AddMinutes(1));
                    var refreshtoken = JwtLibrary.GenerateToken(_options.CurrentValue.ReFreshKey!, newClaims, DateTime.UtcNow.AddDays(7));

                    var hash = new HashEntry[]
                   {
                        new HashEntry("Email",email),
                        new HashEntry("FullName",user.User!.FullName),
                        new HashEntry("Avatar",user.User.Avatar),
                        new HashEntry("State",user.User.State.ToString()),
                        new HashEntry("ReFreshToken",refreshtoken)
                   };

                    await _unitOfWork.accountRepository.UpdateTokenUser(userId, refreshtoken);

                    await _redisService.SetHashValueToRedis(userId, hash);

                    return Result<LoginResponseModel>.Success(new LoginResponseModel
                    {
                        AccountId=userId,
                        AccessToken=accesstoken,
                        ReFreshToken=refreshtoken,
                    });
                }
                

                
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
