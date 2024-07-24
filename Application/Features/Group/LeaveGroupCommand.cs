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

namespace Application.Features.Group
{
    public class LeaveGroupCommand:IRequest<Result<string>>
    {
        public string? Id {  get; set; }
    }
    public class HandLeavGroupCommand : IRequestHandler<LeaveGroupCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _accessor;
        private readonly IHubContext<HubService,IHubServices> _hubContext;
        
        public HandLeavGroupCommand(IUnitOfWork unitOfWork, IHttpContextAccessor accessor, IHubContext<HubService, IHubServices> hubContext)
        {
            _unitOfWork = unitOfWork;
            _accessor = accessor;
            _hubContext = hubContext;
        }

        public async Task<Result<string>> Handle(LeaveGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var User = _accessor.HttpContext!.User.GetUserFromToken();

                var result = await _unitOfWork.groupRepository.LeaveGroup(request.Id!, User.AccountId!);

                if (result.MatchedCount == 0) return Result<string>.Failuer(GroupError.GroupNotFound);

                var message = new Domain.Entities.Message
                {
                    Id=ObjectId.GenerateNewId().ToString(),
                    Content = $"{User.FullName} leaved group!",
                    CreatedAt = DateTime.Now,
                    MessageType=Domain.Enums.MessageType.Notification
                    
                };
                
                await _hubContext.Clients.Group(request.Id!).ReceiveMessage(request.Id!, new object[] { message });

                return Result<string>.Success("Success!");
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
