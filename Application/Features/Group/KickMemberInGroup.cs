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

namespace Application.Features.Group
{
    public class KickMemberInGroupCommand:IRequest<Result<string>>
    {
        public string? Id { get; set; }
        public string? MemberId { get;set; }
    }

    public class HandKickMemberInGroup : IRequestHandler<KickMemberInGroupCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _accessor;
        private readonly IHubContext<HubService,IHubServices> _hubContext;
        public HandKickMemberInGroup(IUnitOfWork unitOfWork, IHttpContextAccessor accessor, IHubContext<HubService, IHubServices> hubContext)
        {
            _unitOfWork = unitOfWork;
            _accessor = accessor;
            _hubContext = hubContext;
        }

        public async Task<Result<string>> Handle(KickMemberInGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var User = await _unitOfWork.groupRepository
                    .FindMemberInGroup(request.Id!, _accessor.HttpContext!.User.GetIdFromClaim());

                var Member = await _unitOfWork.groupRepository.FindMemberInGroup(request.Id!, request.MemberId!);

                if (User is null || Member is null) return Result<string>.Failuer(GroupError.UserNotFound);


                if (User.Role.Equals(GroupRoles.Admin) && Member.Role.Equals(GroupRoles.Member) || User.Role.Equals(GroupRoles.Created))
                {
                    await _unitOfWork.groupRepository.KickMemberInGroup(request.Id!, request.MemberId!);

                    var notification = new Domain.Entities.Notification
                    {
                        Type = NotificationType.ConversationDelete,
                        Content = request.Id
                    };

                    await _hubContext.Clients.Group(request.MemberId!).Notification(notification);

                    return Result<string>.Success("Ok!");
                }


                return Result<string>.Failuer(GroupError.NotPermission);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
