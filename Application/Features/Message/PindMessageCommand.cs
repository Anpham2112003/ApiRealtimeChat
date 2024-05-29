using Domain.Entities;
using Domain.Enums;
using Domain.Errors;
using Domain.Ultils;
using Infrastructure.Services.HubServices;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Message
{
    public class PindMessageCommand:IRequest<Result<string>>
    {
        public string? Id {  get; set; }
        public string? MessageId { get; set; }
        public string? By { get; set; }
        public string? Content {  get; set; }
        public MessageType Type { get; set; }
    }

    public class HandPindMessageCommand : IRequestHandler<PindMessageCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<HubService> _hub;
        private readonly IHttpContextAccessor _httpContext;
        public HandPindMessageCommand(IUnitOfWork unitOfWork, IHubContext<HubService> hub, IHttpContextAccessor httpContext)
        {
            _unitOfWork = unitOfWork;
            _hub = hub;
            _httpContext = httpContext;
        }

        public async Task<Result<string>> Handle(PindMessageCommand request, CancellationToken cancellationToken)
        {
            var UserId = _httpContext.HttpContext!.User.FindFirstValue(ClaimTypes.PrimarySid);
            try
            {
                var message = new PindMessage
                {
                    Id = request.MessageId,
                    AccountId= UserId,
                    By = request.By,
                    Content = request.Content,
                    Type = request.Type
                };

                var result = await _unitOfWork.messageRepository.PindMessage(request.Id!, message);

                if( result.MatchedCount==0) return Result<string>.Failuer(ConversationError.NotFound);

                return Result<string>.Success("Ok");
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
