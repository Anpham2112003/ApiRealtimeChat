using Domain.Entities;
using Domain.Errors;
using Domain.Ultils;
using Infrastructure.Services.HubServices;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Group
{
    public class AddMemberToGroupComand:IRequest<Result<string>>
    {
        public string? Id {  get; set; }
        public List<string>? MemberId { get; set; }

    }

    public class HandAddMemberToGroup : IRequestHandler<AddMemberToGroupComand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _accessor;
        private readonly IHubContext< HubService,IHubServices> _context;
        public HandAddMemberToGroup(IUnitOfWork unitOfWork, IHttpContextAccessor accessor, IHubContext<HubService, IHubServices> context)
        {
            _unitOfWork = unitOfWork;
            _accessor = accessor;
            _context = context;
        }

        public async Task<Result<string>> Handle(AddMemberToGroupComand request, CancellationToken cancellationToken)
        {
            try
            {
                var UserId = _accessor.HttpContext!.User.GetIdFromClaim();

                var result = await _unitOfWork.groupRepository.AddManyMemberToGroup(UserId,request.Id!, request.MemberId!);

                if (result.MatchedCount.Equals(0)) return Result<string>.Failuer(GroupError.GroupNotFound);

                var notification = new Domain.Entities.Notification
                {
                    Type = Domain.Enums.NotificationType.NewConversation,
                    Content = request.Id,
                };
                await _context.Clients.Groups(request.MemberId!).Notification(notification);

                return Result<string>.Success("Ok!");
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
