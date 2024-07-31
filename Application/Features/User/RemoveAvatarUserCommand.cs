using Application.Errors;
using Domain.Entities;
using Domain.ResponeModel;
using Domain.Ultils;
using Infrastructure.Services.AwsService;
using Infrastructure.Services.FileService;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Http;
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
    public class RemoveAvatarUserCommand : IRequest<Result<string>>
    {
        
    }

    public class HandRemoveAvatarUserCommand : IRequestHandler<RemoveAvatarUserCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAwsServices _awsServices;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IFileService _fileService;

        public HandRemoveAvatarUserCommand(IUnitOfWork unitOfWork, IAwsServices awsServices, IConfiguration configuration, IHttpContextAccessor contextAccessor, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _awsServices = awsServices;
            _configuration = configuration;
            _contextAccessor = contextAccessor;
            _fileService = fileService;
        }



        public async Task<Result<string>> Handle(RemoveAvatarUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _contextAccessor.HttpContext!.User.GetIdFromClaim();
               
                var user = await _unitOfWork.userRepository.FindUserByAccountId(userId);

                if (string.IsNullOrEmpty(user!.Avatar)) return Result<string>.Failuer(UserError.AvatarNull);

                //await _awsServices.RemoveFileAsync(_configuration["Aws:Bucket"]!, user.Avatar.Split('/').Last()) ;

                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), user.Avatar);

                _fileService.RemoveFile(imagePath);

                await _unitOfWork.userRepository.RemoveAvatarUser(user.AccountId!);

                return Result<string>.Success(user.Avatar);
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
