using Domain.Entities;
using Domain.Enums;
using Domain.Errors;
using Domain.ResponeModel;
using Domain.Ultils;
using Infrastructure.Services.HubServices;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Message
{
    public class PindMessageCommand:IRequest<Result<string>>
    {
        public string? ConversationId {  get; set; }
        public string? MessageId { get; set; }
       
    }

    public class HandPindMessageCommand : IRequestHandler<PindMessageCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<HubService,IHubServices> _hub;
        private readonly IHttpContextAccessor _httpContext;
        public HandPindMessageCommand(IUnitOfWork unitOfWork, IHubContext<HubService,IHubServices> hub, IHttpContextAccessor httpContext)
        {
            _unitOfWork = unitOfWork;
            _hub = hub;
            _httpContext = httpContext;
        }

        public async Task<Result<string>> Handle(PindMessageCommand request, CancellationToken cancellationToken)
        {
           
            try
            {
                var User = _httpContext.HttpContext!.User.GetUserFromToken();
               
                var findMessage =await _unitOfWork.messageRepository.FindMessage(request.ConversationId!,request.MessageId!);

                if (findMessage is null || findMessage.IsDelete==true || findMessage.AccountId is null )
                    return Result<string>.Failuer(new Error("Fail", "Cannot Pind Message"));

                var pindMessage = new ClientMessageResponseModel
                {
                   AccountId= User.AccountId,
                   Content = findMessage.Content,
                   CreatedAt = DateTime.UtcNow,
                   Id=request.MessageId,
                   MessageType=MessageType.PindMessage,
                   UpdatedAt= DateTime.UtcNow,
                   User= User,
            
                };
                var notification = new Domain.Entities.Message
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    MessageType = MessageType.Notification,
                    Content = $"{User.FullName} pinned message!",
                    CreatedAt = DateTime.UtcNow,
                };

                var result = await _unitOfWork.messageRepository.PindMessage(request.ConversationId!,findMessage);

                await _unitOfWork.messageRepository.SendMessageAsync(request.ConversationId!,User.AccountId!,notification);

                await _hub.Clients.Group(request.ConversationId!).ReceiveMessage(request.ConversationId!, new object[] {notification,pindMessage});



                return Result<string>.Success("Ok");
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
