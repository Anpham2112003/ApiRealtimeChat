using Domain.Entities;
using Domain.Enums;
using Domain.Errors;
using Domain.ResponeModel;
using Domain.Ultils;
using Infrastructure.Services;
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
        private readonly IAwsServices _awsServices;
        private readonly IHubContext<HubService,IHubServices> _hubContext;
        private readonly IConfiguration _configuration;
        public HandSendFileCommand(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor, IAwsServices awsServices, IHubContext<HubService, IHubServices> hubContext, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
            _awsServices = awsServices;
            _hubContext = hubContext;
            _configuration = configuration;
        }

        public async Task<Result<string>> Handle(SendFileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = _contextAccessor.HttpContext!.User.GetUserFromToken();

                var url = new StringBuilder();
                url.Append("File")
                    .Append(Random.Shared.Next(1, 1000000000))
                    .Append(ObjectId.GenerateNewId())
                    .Append(Path.GetExtension(request.File!.FileName));


                var AwsPath = _configuration["Aws:Perfix"] + url;


                var message = new Domain.Entities.Message
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    AccountId = user.AccountId,
                    Content = AwsPath,
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

                await _awsServices.UploadFileAsync(_configuration["Aws:Bucket"]!, url.ToString(), request.File);

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
