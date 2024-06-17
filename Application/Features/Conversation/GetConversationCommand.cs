using Domain.Entities;
using Domain.ResponeModel.BsonConvert;
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

namespace Application.Features.Conversation
{
    public class GetConversationCommand:IRequest<Result<ConversationConvert>>
    {
        public string? Id {  get; set; }
    }

    public class HandGetConversationSingleChatCommand : IRequestHandler<GetConversationCommand, Result<ConversationConvert>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor  _accessor;
        private readonly IHubContext<HubService,IHubServices> _hubContext;
        public HandGetConversationSingleChatCommand(IUnitOfWork unitOfWork, IHttpContextAccessor accessor, IHubContext<HubService, IHubServices> hubContext)
        {
            _unitOfWork = unitOfWork;
            _accessor = accessor;
            _hubContext = hubContext;
        }

        public async Task<Result<ConversationConvert>> Handle(GetConversationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var ToId = request.Id;

                var FromId = _accessor.HttpContext!.User.FindFirstValue(ClaimTypes.PrimarySid);

                var conversation = await _unitOfWork.conversationRepository.GetConversation(FromId, ToId!);
                
                if(conversation is null)
                {
                    var newConversation = new ConversationCollection
                    {
                        Id=ObjectId.GenerateNewId().ToString(),
                        Messages=new List<Domain.Entities.Message> { },
                        CreatedAt = DateTime.UtcNow,
                        MessagePinds= new List<ObjectId> { },
                        IsGroup = false,
                        Owners = new List<ObjectId>() {ObjectId.Parse(FromId),ObjectId.Parse(ToId) },
                        Seen=DateTime.UtcNow,
                        
                    };

                  
                    
                     await  _unitOfWork.conversationRepository.InsertAsync(newConversation);

                    var notification = new Domain.Entities.Notification
                    {
                        Type = Domain.Enums.NotificationType.NewConversation,
                        Content = newConversation.Id,
                    };

                    await _hubContext.Clients.Groups(FromId,ToId!).Notification(notification);
                    
                    return Result<ConversationConvert>.Success(new ConversationConvert { Id=newConversation.Id});
                }

                return Result<ConversationConvert>.Success(conversation);
            }
            catch (Exception)
            {

                throw;
            }
            
        }
    }
}
