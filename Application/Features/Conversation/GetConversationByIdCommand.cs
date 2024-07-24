using Domain.Entities;
using Domain.Errors;
using Domain.ResponeModel;
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
    public class GetConversationByIdCommand:IRequest<Result<ConversationResponseModel>>
    {       
        public string? Id { get; set; }
    }

    public class HandGetConversatiomByIdCommand : IRequestHandler<GetConversationByIdCommand, Result<ConversationResponseModel>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        public HandGetConversatiomByIdCommand(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<ConversationResponseModel>> Handle(GetConversationByIdCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var UserId = _contextAccessor.HttpContext!.User.GetIdFromClaim();

                var result = await _unitOfWork.conversationRepository.GetConversationByIdAsync(request.Id!, UserId);

                if (result is null) return Result<ConversationResponseModel>.Failuer(ConversationError.NotFound);

                return Result<ConversationResponseModel>.Success(result);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
