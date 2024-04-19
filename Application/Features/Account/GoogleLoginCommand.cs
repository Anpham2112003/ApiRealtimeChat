﻿using Application.ResponeModel;
using Application.Ultils;
using Domain.Entites;
using Domain.Settings;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
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

        public HandGoogleLoginCommnand(IUnitOfWork unitOfWork, IOptionsMonitor<JwtSetting> options)
        {
            _unitOfWork = unitOfWork;
            _options = options;
        }

        public async Task<Result<LoginResponseModel>> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var check = await _unitOfWork.accountRepository.FindAccountByEmail("Google:" + request.Email!);
                if (check is null)
                {
                    var account = new AccountCollection()
                    {
                        Id = request.Id,
                        Email = "Google:" + request.Email,
                        AccountSate = Domain.Enums.AccountSate.Offline,
                        AccountType = Domain.Enums.AccountType.Google,
                        CreatedAt = DateTime.UtcNow,
                        IsDelete = false,
                    };

                    var user = new UserCollection()
                    {

                        Id = ObjectId.GenerateNewId(),
                        AccountId = request.Id,
                        FistName = request.FistName,
                        LastName = request.LastName,
                        FullName = request.FistName + " " + request.LastName

                    };

                    var addAccount = _unitOfWork.accountRepository.InsertAsync(account);

                    var addUser = _unitOfWork.userRepository.InsertUserAsync(user);

                    await Task.WhenAll(addAccount, addUser);

                    var claims = new[]
                    {
                    new Claim(ClaimTypes.Email,account.Email),
                    new Claim(ClaimTypes.UserData,account.Id.ToString())
                };

                    var accessToken = JwtLibrary.GenerateToken(_options.CurrentValue.AccessKey!, claims, DateTime.UtcNow.AddMinutes(1));
                    var refeshToken = JwtLibrary.GenerateToken(_options.CurrentValue.ReFreshKey!, claims, DateTime.UtcNow.AddMinutes(1));

                    return Result<LoginResponseModel>.Success(new LoginResponseModel(account.Id.ToString(), accessToken, refeshToken));
                }
                else
                {

                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Email,check.Email!),
                        new Claim(ClaimTypes.UserData,check.Id.ToString())
                    };

                    var accessToken = JwtLibrary.GenerateToken(_options.CurrentValue.AccessKey!, claims, DateTime.UtcNow.AddMinutes(1));
                    var refeshToken = JwtLibrary.GenerateToken(_options.CurrentValue.ReFreshKey!, claims, DateTime.UtcNow.AddMinutes(1));

                    return Result<LoginResponseModel>.Success(new LoginResponseModel(check.Id.ToString(), accessToken, refeshToken));
                }
            }
            catch (Exception)
            {

                throw;
            }
            
        }
    }
}
