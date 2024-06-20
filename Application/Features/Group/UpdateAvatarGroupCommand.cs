using Domain.Errors;
using Domain.Ultils;
using Infrastructure.Services;
using Infrastructure.Services.HubServices;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
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
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IHubContext<HubService,IHubServices> _hubContext;
        public HandUpdateAvatarGroupCommand(IUnitOfWork unitOfWork, IAwsServices awsServices, IConfiguration configuration, IHttpContextAccessor contextAccessor, IHubContext<HubService, IHubServices> hubContext)
        {
            _unitOfWork = unitOfWork;
            _awsServices = awsServices;
            this.configuration = configuration;
            _contextAccessor = contextAccessor;
            _hubContext = hubContext;
        }

        public async Task<Result<string>> Handle(UpdateAvatarGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var User = _contextAccessor.HttpContext!.User.GetUserFromToken();

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
                
                var message = new Domain.Entities.Message
                {
                    Content=$"{User.Name} was change avatar Group!",
                    MessageType=Domain.Enums.MessageType.Notification,
                    CreatedAt=DateTime.UtcNow,  
                };

                await _unitOfWork.messageRepository.SendMessageAsync(request.Id!,User.AccountId!,message);

                await _hubContext.Clients.Groups(request.Id!).ReceiveMessage(request.Id!,message);

                return Result<string>.Success(url);
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
