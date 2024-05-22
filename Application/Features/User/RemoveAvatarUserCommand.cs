using Application.Errors;
using Domain.Entities;
using Domain.ResponeModel;
using Domain.Ultils;
using Infrastructure.Services;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.User
{
    public class RemoveAvatarUserCommand : IRequest<Result<UploadAvatarResponeModel>>
    {
        public string? AccountId { get; set; }

        public RemoveAvatarUserCommand(string? accountId)
        {
            AccountId = accountId;
        }
    }

    public class HandRemoveAvatarUserCommand : IRequestHandler<RemoveAvatarUserCommand, Result<UploadAvatarResponeModel>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAwsServices _awsServices;
        private readonly IConfiguration _configuration;

        public HandRemoveAvatarUserCommand(IUnitOfWork unitOfWork, IAwsServices awsServices, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _awsServices = awsServices;
            _configuration = configuration;
        }



        public async Task<Result<UploadAvatarResponeModel>> Handle(RemoveAvatarUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _unitOfWork.userRepository.FindUserByAccountId(request.AccountId!);

                if (user is null) return Result<UploadAvatarResponeModel>.Failuer(UserError.UserNotFound(request.AccountId!));

                if (user.Avatar is null) return Result<UploadAvatarResponeModel>.Failuer(UserError.AvatarNull);

                await _awsServices.RemoveFileAsync(_configuration["Aws:Bucket"]!, user.Avatar);

                var filter = Builders<UserCollection>.Filter.Eq(x => x.AccountId, request.AccountId!);

                var update = Builders<UserCollection>.Update.Set(x => x.Avatar, null);
               

                await _unitOfWork.userRepository.UpdateAsync(filter, update);

                return Result<UploadAvatarResponeModel>.Success(new UploadAvatarResponeModel("Remove success!", request.AccountId, user.Avatar));
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
