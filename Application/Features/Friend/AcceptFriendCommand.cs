using Application.Errors;
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

namespace Application.Features.Friend
{
    public class AcceptFriendCommand:IRequest<Result<Object>>
    {
        
        public string? Id { get; set; }

        public AcceptFriendCommand(string? id)
        {
            Id = id;
        }
    }

    public class HandAddFriendCommand : IRequestHandler<AcceptFriendCommand, Result<Object>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IHubContext<HubService, IHubServices> _hubContext;
        public HandAddFriendCommand(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor, IHubContext<HubService, IHubServices> hubContext)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
            _hubContext = hubContext;
        }

        public async Task<Result<object>> Handle(AcceptFriendCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var User = _contextAccessor.HttpContext?.User.GetUserFromToken();

                var friend = await _unitOfWork.accountRepository.CheckExist(x=>x.Id==request.Id);

                if ( friend is false) return Result<Object>.Failuer(FriendError.DocumentNotFound);

                var notificaton = new Domain.Entities.Notification
                {
                     Id=ObjectId.GenerateNewId().ToString(),
                     Content=$"{User!.FullName} accepted the friend request!",
                     From=User.AccountId,
                     CreatedAt=DateTime.UtcNow,
                     Type=Domain.Enums.NotificationType.AppcectFriend
                };

                var hasInviteList = await _unitOfWork.friendRepository.HasInInviteList(User.AccountId!,request.Id!);

                 

                if (hasInviteList)
                {
                    var result = await _unitOfWork.friendRepository.AcceptFriend(User.AccountId!, request.Id!);

                    await _unitOfWork.notificationRepository.AddNotification(request.Id!, notificaton);

                    await _unitOfWork.notificationRepository.RemoveNotification(User.AccountId!,request.Id!,Domain.Enums.NotificationType.InviteFriend);

                    await _hubContext.Clients.User(request.Id!).Notification(notificaton);

                    return Result<Object>.Success();
                }

                return Result<Object>.Failuer(new Error("Not in InviteList!", "Bad"));
            }
            catch (Exception)
            {

                throw;
            }
            
        }
    }
}
