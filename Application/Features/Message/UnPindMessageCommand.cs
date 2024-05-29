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
    public class UnPindMessageCommand:IRequest<Result<string>>
    {
        public string? Id {  get; set; }
        public string? MessageId {  get; set; }
    }

    public class HandUnPindMessageCommand : IRequestHandler<UnPindMessageCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContext;

        public HandUnPindMessageCommand(IUnitOfWork unitOfWork, IHttpContextAccessor httpContext)
        {
            _unitOfWork = unitOfWork;
            _httpContext = httpContext;
        }

        public async Task<Result<string>> Handle(UnPindMessageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var UserId = _httpContext.HttpContext!.User.GetIdFromClaim();

                var result = await _unitOfWork.messageRepository.UnPindMessage(request.Id!,UserId, request.MessageId!);

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
