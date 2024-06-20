using Application.Errors;
using Domain.Enums;
using Domain.Ultils;
using Infrastructure.Services.HubServices;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Friend
{
    public class SendInviteFriendCommand:IRequest<Result<string>>
    {
        public string? Id { get; set; }
    }

    public class HandAddToWaitListCommand : IRequestHandler<SendInviteFriendCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IHubContext<HubService, IHubServices> _hubContext;

        public HandAddToWaitListCommand(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor, IHubContext<HubService, IHubServices> hubContext)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
            _hubContext = hubContext;
        }

        public async Task<Result<string>> Handle(SendInviteFriendCommand request, CancellationToken cancellationToken)
        {
            try
            {

                var User = _contextAccessor.HttpContext!.Request.HttpContext.User.GetUserFromToken();


                var checkFriendAccount = await _unitOfWork.accountRepository.CheckAccountExist(request.Id!);

                if ( !checkFriendAccount)

                return Result<string>.Failuer(FriendError.DocumentNotFound);

                await _unitOfWork.friendRepository.AddToWaitlistAsync( request.Id!,User.AccountId!) ;

                var notification = new Domain.Entities.Notification()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Content = $"{User.Name} Send a friend request to you!",
                    From = User.AccountId,
                    CreatedAt = DateTime.UtcNow,
                    Type = NotificationType.InviteFriend,
                };
                await _hubContext.Clients.Group(request.Id!).Notification(notification);

                await _unitOfWork.notificationRepository.AddNotification(request.Id!, notification);

                return Result<string>.Success("Success!");
            }
            catch (Exception)
            {

                throw;
            }
            
        }
    }
}
