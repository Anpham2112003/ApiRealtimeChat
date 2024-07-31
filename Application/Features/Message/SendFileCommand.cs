using Domain.Entities;
using Domain.Enums;
using Domain.Errors;
using Domain.ResponeModel;
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

namespace Application.Features.Message
{
    public class SendFileCommand:IRequest<Result<string>>
    {
        public string? Id {  get; set; }
        public FileType Type { get; set; }
        public IFormFile? File { get; set; }
    }

    public class HandSendFileCommand : IRequestHandler<SendFileCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IHubContext<HubService,IHubServices> _hubContext;
        private readonly IFileService _fileService;
        public HandSendFileCommand(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor,  IHubContext<HubService, IHubServices> hubContext, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
            _hubContext = hubContext;
            _fileService = fileService;
        }

        public async Task<Result<string>> Handle(SendFileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = _contextAccessor.HttpContext!.User.GetUserFromToken();

                if (request.File ==null) return Result<string>.Failuer(new Error("File null", "File Null"));

                var fileName =Path.GetRandomFileName()+ Path.GetExtension(request.File!.FileName);

                var url = new StringBuilder();

                if (request.Type == FileType.Image)
                {
                    url.Append(Path.Combine( "Uploads", "Images"));

                }

                else if (request.Type == FileType.File)
                {
                    url.Append(Path.Combine("Uploads", "Files"));
                }

                else if (request.Type == FileType.Video)
                {
                    url.Append(Path.Combine( "Uploads", "Videos"));
                }

                else
                {
                    throw new Exception("File type upload invalid");
                }
                    
              


                var message = new Domain.Entities.Message
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    AccountId = user.AccountId,
                    Content =  Path.Combine(url.ToString(),fileName),
                    MessageType = (MessageType)request.Type,
                    CreatedAt = DateTime.UtcNow,
                    IsDelete = false,
                    
                };

                var result = await _unitOfWork.messageRepository.SendMessageAsync(request.Id!, user.AccountId!, message);


                if (result.ModifiedCount == 0) return Result<string>.Failuer(ConversationError.NotFound);

                var messageReceiver = new ClientMessageResponseModel
                {
                    Id = message.Id,
                    AccountId = message.AccountId,
                    Content = message.Content,
                    CreatedAt = DateTime.UtcNow,
                    IsDelete = false,
                    MessageType = MessageType.Message,
                    User = user
                };


                await _fileService.WriteFileAsync(url.ToString(), fileName, request.File);

                await _hubContext.Clients.Groups(request.Id!).ReceiveMessage(request.Id!, new object[] {messageReceiver});

                return Result<string>.Success("Ok");
            }
            catch (Exception)
            {

                throw;
            }
            
        }
    }
}
