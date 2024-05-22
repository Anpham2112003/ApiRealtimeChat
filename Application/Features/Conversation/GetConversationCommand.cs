using Domain.Entities;
using Domain.ResponeModel.BsonConvert;
using Domain.Ultils;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Conversation
{
    public class GetConversationCommand:IRequest<Result<ConversationCollection>>
    {
        public string? Id {  get; set; }
    }

    public class HandGetConversationSingleChatCommand : IRequestHandler<GetConversationCommand, Result<ConversationCollection>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor  _accessor;
        public HandGetConversationSingleChatCommand(IUnitOfWork unitOfWork, IHttpContextAccessor accessor)
        {
            _unitOfWork = unitOfWork;
            _accessor = accessor;
        }

        public async Task<Result<ConversationCollection>> Handle(GetConversationCommand request, CancellationToken cancellationToken)
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
                        IsGroup = false,
                        Owners = new List<string>() {FromId,ToId! }
                    };
                    
                     await  _unitOfWork.conversationRepository.InsertAsync(newConversation);
                    
                    return Result<ConversationCollection>.Success(newConversation);
                }

                return Result<ConversationCollection>.Success(conversation);
            }
            catch (Exception)
            {

                throw;
            }
            
        }
    }
}
