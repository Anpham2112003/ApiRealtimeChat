using Domain.Errors;
using Domain.Ultils;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Conversation
{
    public class RemoveConversationCommand:IRequest<Result<string>>
    {
        public string? Id { get; set; }
    }

    public class HandRemoveConversation : IRequestHandler<RemoveConversationCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;

        public HandRemoveConversation(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<string>> Handle(RemoveConversationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var UserId = _contextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.PrimarySid);

                var conversation = await _unitOfWork.conversationRepository.GetInforConversation(UserId, request.Id!);

                if (conversation is null) return Result<string>.Failuer(ConversationError.NotFound);

                if (conversation.IsGroup == true)
                {
                    await _unitOfWork.groupRepository.LeaveGroup(request.Id!, UserId);
                }

                await _unitOfWork.conversationRepository.RemoveConversation(conversation.Id!);

                return Result<string>.Success(request.Id);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
