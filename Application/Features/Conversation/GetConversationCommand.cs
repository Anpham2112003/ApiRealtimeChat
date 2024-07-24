using Domain.Entities;
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

namespace Application.Features.Conversation
{
    public class GetConversationCommand:IRequest<Result<ConversationResponseModel>>
    {
        public string? Id {  get; set; }
    }

    public class HandGetConversationSingleChatCommand : IRequestHandler<GetConversationCommand, Result<ConversationResponseModel>>
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

        public async Task<Result<ConversationResponseModel>> Handle(GetConversationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                

                var UserId = _accessor.HttpContext!.User.GetIdFromClaim();

                var checkUserExist = await _unitOfWork.accountRepository.CheckExist(x=>x.Id==request.Id);

                if (!checkUserExist) return Result<ConversationResponseModel>.Failuer(new Error("NotFound",""));
                
                var conversation = await _unitOfWork.conversationRepository.GetConversation(UserId, request.Id!);
                
                if(conversation is null)
                {
                    var newConversation = new ConversationCollection
                    {
                        Id=ObjectId.GenerateNewId().ToString(),
                        Messages=new List<Domain.Entities.Message> { },
                        CreatedAt = DateTime.UtcNow,
                        Pinds= new List<Domain.Entities.Message> { },
                        IsGroup = false,
                        Owners = new List<ObjectId>() {ObjectId.Parse(UserId),ObjectId.Parse(request.Id!) },
                        Seen=DateTime.UtcNow,
                        
                    };

                  
                    
                    await  _unitOfWork.conversationRepository.InsertAsync(newConversation);

                    var notification = new Domain.Entities.Notification
                    {
                        Type = Domain.Enums.NotificationType.NewConversation,
                        Content = newConversation.Id,
                        
                    };

                    await _hubContext.Clients.Groups(UserId,request.Id!).Notification(notification);
                    
                    return Result<ConversationResponseModel>.Success(new ConversationResponseModel { Id=newConversation.Id});
                }

                return Result<ConversationResponseModel>.Success(conversation);
            }
            catch (Exception)
            {

                throw;
            }
            
        }
    }
}
