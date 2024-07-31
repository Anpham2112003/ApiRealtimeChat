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
    public class ChangeContentMessageCommand:IRequest<Result<string>>
    {
        public string? Id {  get; set; }
        public string? MessageId { get; set; }
        public string? Content {  get; set; }
    }

    public class HandChangeContentMessageCommand : IRequestHandler<ChangeContentMessageCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContext;

        public HandChangeContentMessageCommand(IUnitOfWork unitOfWork, IHttpContextAccessor httpContext)
        {
            _unitOfWork = unitOfWork;
            _httpContext = httpContext;
        }

        public async Task<Result<string>> Handle(ChangeContentMessageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var UserId = _httpContext.HttpContext!.User.GetIdFromClaim();

                var result = await _unitOfWork.messageRepository.ChangeContentMessage(request.Id!, UserId, request.MessageId!, request.Content!);

                if (result.MatchedCount == 0) return Result<string>.Failuer(new Error("Not found","Conversation not found or Message not exist in List Message"));

                return Result<string>.Success(request.MessageId);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
