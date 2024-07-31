using Domain.Enums;
using Domain.Errors;
using Domain.ResponeModel;
using Domain.Ultils;
using Infrastructure.Services.HubServices;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Message
{
    public class RemoveMessageCommand:IRequest<Result<string>>
    {
        public string? ConversationId { get; set; }
        public string? MessageId { get; set;}
    }

    public class HandRemoveMessageCommand : IRequestHandler<RemoveMessageCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHubContext<HubService,IHubServices> _hubContext ;
        public HandRemoveMessageCommand(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IHubContext<HubService, IHubServices> hubContext)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _hubContext = hubContext;
        }

        public async Task<Result<string>> Handle(RemoveMessageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var UserId = _httpContextAccessor.HttpContext!.User.GetIdFromClaim();

                var result = await _unitOfWork.messageRepository.RemoveMessage(request.ConversationId!, UserId, request.MessageId!);

                if (result is null ||  !result.Any() ) return Result<string>.Failuer(ConversationError.NotFound);

                var eventMessage = new Event
                {
                     EventType= EventType.RemoveMessage,
                     EventMessage = $"{request.ConversationId}:{request.MessageId}"
                };

                await _hubContext.Clients.Users(result).Event(eventMessage);

                return Result<string>.Success("Ok!");
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
