using Application.Errors;
using Application.Ultils;
using Infrastructure.Services;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Group
{
    public class RemoveAvatarGroupCommand:IRequest<Result<string>>
    {
        public string? GroupId { get; set; }

    }
    public class HandRemoveAvartaGroupCommand : IRequestHandler<RemoveAvatarGroupCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IAwsServices _awsServices;
        public HandRemoveAvartaGroupCommand(IUnitOfWork unitOfWork, IConfiguration configuration, IAwsServices awsServices)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _awsServices = awsServices;
        }

        public async Task<Result<string>> Handle(RemoveAvatarGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var groupId = ObjectId.Parse(request.GroupId);

                var group = await _unitOfWork.groupRepository.GetAvatarGroupAsync(groupId);

                if (group == null) return Result<string>.Failuer(GroupError.NotFound);

                await _awsServices.RemoveFileAsync(_configuration["Aws:Bucket"]!, group.Avatar!);

                await _unitOfWork.groupRepository.UpdateAvatarGroupAsync(groupId, "");

                return Result<string>.Success("success!");
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
