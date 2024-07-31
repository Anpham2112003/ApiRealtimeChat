using Domain.Errors;
using Domain.Ultils;
using Infrastructure.Services;
using Infrastructure.Services.FileService;
using Infrastructure.Services.HubServices;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
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
        //private readonly IAwsServices _awsServices;
        //private readonly IConfiguration configuration;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IHubContext<HubService,IHubServices> _hubContext;
        private readonly IFileService _fileService;
        public HandUpdateAvatarGroupCommand(IUnitOfWork unitOfWork/*, IAwsServices awsServices, IConfiguration configuration,*/, IHttpContextAccessor contextAccessor, IHubContext<HubService, IHubServices> hubContext, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            //_awsServices = awsServices;
            //this.configuration = configuration;
            _contextAccessor = contextAccessor;
            _hubContext = hubContext;
            _fileService = fileService;
        }

        public async Task<Result<string>> Handle(UpdateAvatarGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var User = _contextAccessor.HttpContext!.User.GetUserFromToken();

                if (request.File == null) return Result<string>.Failuer(new Error("File", "File null"));

                var fileName = ObjectId.GenerateNewId()+Path.GetExtension(request.File!.FileName);

                //await _awsServices.UploadFileAsync(configuration["Aws:Bucket"]!, fileName.ToString(), request.File!);

                //var url = configuration["Aws:Perfix"] + fileName.ToString();

                var storePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Images");

                var url = Path.Combine("Uploads", "Images", fileName);

                await _fileService.WriteFileAsync(storePath,fileName, request.File);

                var result = await _unitOfWork.groupRepository.UpdateAvatarGroupAsync(request.Id!, url);

                if(result.MatchedCount.Equals(0)) return Result<string>.Failuer(GroupError.GroupNotFound);
                
                var message = new Domain.Entities.Message
                {
                    Content=$"{User.FullName} was change avatar Group!",
                    MessageType=Domain.Enums.MessageType.Notification,
                    CreatedAt=DateTime.UtcNow,  
                };

                await _unitOfWork.messageRepository.SendMessageAsync(request.Id!,User.AccountId!,message);

                await _hubContext.Clients.Groups(request.Id!).ReceiveMessage(request.Id!,new object[] { message });

                return Result<string>.Success(url);
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
