using Application.Errors;
using Application.Ultils;
using Domain.Entites;
using Infrastructure.Services;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Group
{
    public class UpdateAvatarGroupCommand:IRequest<Result<string>>
    {
        public string? GroupId {  get; set; }
        public IFormFile? File { get; set; }
    }

    public class HandUpdateAvartaGroupCommand : IRequestHandler<UpdateAvatarGroupCommand, Result<string>>
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAwsServices _awsServices;
        private readonly IConfiguration _configuration;
        public HandUpdateAvartaGroupCommand(IHttpContextAccessor contextAccessor, IUnitOfWork unitOfWork, IAwsServices awsServices, IConfiguration configuration)
        {
            _contextAccessor = contextAccessor;
            _unitOfWork = unitOfWork;
            _awsServices = awsServices;
            _configuration = configuration;
        }

        public async Task<Result<string>> Handle(UpdateAvatarGroupCommand request, CancellationToken cancellationToken)
        {
            var userId = ObjectId.Parse(_contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.PrimarySid));

            var groupId=ObjectId.Parse(request.GroupId);

            var check = await _unitOfWork.groupRepository.CheckMemberInGroupAsync(groupId, userId);

            if (check == null) return Result<string>.Failuer(GroupError.NotFound);

            var avatarKey = "AvatarGroup" + groupId.ToString() + DateTime.UtcNow;

            await _awsServices.UploadFileAsync(_configuration["Aws:Bucket"]!, avatarKey, request.File!);

            await _unitOfWork.groupRepository.UpdateAvatarGroupAsync(groupId,  avatarKey);
           
            return Result<string>.Success(_configuration["Aws:Bucket"]+avatarKey);
        }
    }
}
