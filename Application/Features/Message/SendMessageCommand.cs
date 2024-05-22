using Domain.Entities;
using Domain.Enums;
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
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Message
{
    public class SendMessageCommand:IRequest<Result<string>>
    {
        public string? Id {  get; set; }
        public MessageType Type { get; set; }
        public string? Content {  get; set; }
        public IFormFile? File { get; set; }
    }
    public class HandSendMessageCommand : IRequestHandler<SendMessageCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IHubContext<HubService,IHubServices> _hub;
        public HandSendMessageCommand(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor, IHubContext<HubService> hub)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
            _hub = hub;
        }

        public async Task<Result<string>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var UserId = _contextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.PrimarySid);

                var message = new Domain.Entities.Message
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    UserId = UserId,
                    Content = request.Content,
                    MessageType = MessageType.Message,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _unitOfWork.messageRepository.SendMessageAsync(request.Id!, UserId, message);

                await _hub.Clients.Groups(request.Id!).SendMessage(request.Id!, message);

                if (result.ModifiedCount == 0) return Result<string>.Failuer(ConversationError.NotFound);

                return Result<string>.Success("Ok");
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
