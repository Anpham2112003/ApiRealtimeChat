using Domain.Errors;
using Domain.Ultils;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Message
{
    public class RemoveMessageCommand:IRequest<Result<string>>
    {
        public string? Id { get; set; }
        public string? MessageId { get; set;}
    }

    public class HandRemoveMessageCommand : IRequestHandler<RemoveMessageCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HandRemoveMessageCommand(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<string>> Handle(RemoveMessageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var UserId = _httpContextAccessor.HttpContext!.User.GetIdFromClaim();

                var result = await _unitOfWork.messageRepository.RemoveMessage(request.Id!, UserId, request.MessageId!);

                if (result.MatchedCount == 0) return Result<string>.Failuer(ConversationError.NotFound);

                return Result<string>.Success("Ok!");
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
