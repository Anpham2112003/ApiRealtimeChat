using Domain.Errors;
using Domain.Ultils;
using Infrastructure.Services;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Group
{
    public class UpdateAvatarGroupCommand:IRequest<Result<string>>
    {
        public string? Id {  get; set; }
        public IFormFile? File { get; set; }
    }

    public class HandUpdateAvatarGroupCommand : IRequestHandler<UpdateAvatarGroupCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAwsServices _awsServices;
        private readonly IConfiguration configuration;
        public HandUpdateAvatarGroupCommand(IUnitOfWork unitOfWork, IAwsServices awsServices, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _awsServices = awsServices;
            this.configuration = configuration;
        }

        public async Task<Result<string>> Handle(UpdateAvatarGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                StringBuilder urlBildder = new StringBuilder();

                urlBildder
                  .Append("GroupAvatar")
                  .Append(request.Id)
                  .Append(request.File!.FileName)
                  .Append(Random.Shared.Next(100000000))
                  .Append(Path.GetExtension(request.File.FileName));


                await _awsServices.UploadFileAsync(configuration["Aws:Bucket"]!, urlBildder.ToString(), request.File);

                var url = configuration["Aws:Perfix"] + urlBildder.ToString();

                var result = await _unitOfWork.groupRepository.UpdateAvatarGroupAsync(request.Id!, url);

                if(result.MatchedCount.Equals(0)) return Result<string>.Failuer(GroupError.GroupNotFound);

                return Result<string>.Success(url);
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
