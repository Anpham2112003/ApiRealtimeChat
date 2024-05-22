using Domain.Entities;
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
    public class GetAllConversationCommand:IRequest<Result<List<ConversationCollection>>>
    {
        public int skip {  get; set; }
        public int limit { get; set; }  
    }

    public class HandGetAllConversation : IRequestHandler<GetAllConversationCommand, Result<List<ConversationCollection>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;

        public HandGetAllConversation(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<List<ConversationCollection>>> Handle(GetAllConversationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var UserId = _contextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.PrimarySid);

                var result = await _unitOfWork.conversationRepository.GetAllConversationAsync(UserId, request.skip, request.limit);

                if (!result.Any()) return Result<List<ConversationCollection>>.Failuer(ConversationError.NotFound);

                return Result<List<ConversationCollection>>.Success(result);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
