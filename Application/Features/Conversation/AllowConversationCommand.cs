using Domain.Ultils;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Conversation
{
    public class AllowConversationCommand:IRequest<Result<string>>
    {
        public string? ConversationId { get; set; }
    }

    public class HandAllowConversation : IRequestHandler<AllowConversationCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;

        public HandAllowConversation(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<string>> Handle(AllowConversationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var myId = _contextAccessor.HttpContext!.User.GetIdFromClaim();

                var result = await _unitOfWork.conversationRepository.AllowConversation(request.ConversationId!, myId);

                if (result.ModifiedCount == 0) return Result<string>.Failuer(new Error("Not Found", ""));

                return Result<string>.Success("Ok!");
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
