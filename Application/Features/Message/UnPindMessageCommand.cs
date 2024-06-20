using Domain.Errors;
using Domain.Ultils;
using Infrastructure.Services.HubServices;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Message
{
    public class UnPindMessageCommand:IRequest<Result<string>>
    {
        public string? ConversationId {  get; set; }
        public string? MessageId {  get; set; }
    }

    public class HandUnPindMessageCommand : IRequestHandler<UnPindMessageCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IHubContext<HubService,IHubServices> _hubContext;
        public HandUnPindMessageCommand(IUnitOfWork unitOfWork, IHttpContextAccessor httpContext, IHubContext<HubService, IHubServices> hubContext)
        {
            _unitOfWork = unitOfWork;
            _httpContext = httpContext;
            _hubContext = hubContext;
        }

        public async Task<Result<string>> Handle(UnPindMessageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var User = _httpContext.HttpContext!.User.GetUserFromToken();

                var result = await _unitOfWork.messageRepository.UnPindMessage(request.ConversationId!,User.AccountId!, request.MessageId!);

                if (result.MatchedCount == 0) return Result<string>.Failuer(ConversationError.NotFound);

                var message = new Domain.Entities.Message
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    MessageType = Domain.Enums.MessageType.UnPindMessage,
                    Content = request.MessageId,
                    
                };
                var notification = new Domain.Entities.Message
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    MessageType = Domain.Enums.MessageType.Notification,
                    Content = $"{User.Name} unpinded message",
                    CreatedAt = DateTime.UtcNow,
                };



                await _unitOfWork.messageRepository.SendMessageAsync(request.ConversationId!,User.AccountId!, notification);

                await _hubContext.Clients.Group(request.ConversationId!).ReceiveMessage(request.ConversationId!,message);

                await _hubContext.Clients.Group(request.ConversationId!).ReceiveMessage(request.ConversationId!, notification);

                return Result<string>.Success("Ok!");
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
