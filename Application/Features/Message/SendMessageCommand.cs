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
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Message
{
    public class SendMessageCommand:IRequest<Result<string>>
    {
        public string? Id {  get; set; }
        public string? Content {  get; set; }
        
    }
    public class HandSendMessageCommand : IRequestHandler<SendMessageCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IHubContext<HubService,IHubServices> _hub;
       
        public HandSendMessageCommand(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor, IHubContext<HubService, IHubServices> hub)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
            _hub = hub;
           
        }

        public async Task<Result<string>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            try
            {

                    var User = _contextAccessor.HttpContext!.User.GetUserFromToken();


                    var message = new Domain.Entities.Message
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        AccountId = User.AccountId,
                        Content = request.Content,
                        MessageType = MessageType.Message,
                        CreatedAt = DateTime.UtcNow,
                    };

                    var result = await _unitOfWork.messageRepository.SendMessageAsync(request.Id!, User.AccountId!, message);

                    if (result.ModifiedCount == 0) return Result<string>.Failuer(ConversationError.NotFound);

                var messageReceiver = new ClientMessageResponseModel
                {
                    Id = message.Id,
                    AccountId = message.AccountId,
                    Content = message.Content,
                    CreatedAt = DateTime.UtcNow,
                    IsDelete = false,
                    MessageType = MessageType.Message,
                    User = User

                };

                    await _hub.Clients.Group(request.Id!).ReceiveMessage(request.Id!, new object[] { messageReceiver });

                    return Result<string>.Success("Ok");
                
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
