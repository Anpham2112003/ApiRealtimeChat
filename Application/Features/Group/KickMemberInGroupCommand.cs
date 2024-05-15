using Application.Errors;
using Application.Ultils;
using Domain.Enums;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
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
        public string? GroupId { get; set; }
        public string? MemberId { get; set; }
    }

    public class HandRemoveMemberInGroup : IRequestHandler<KickMemberInGroupCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        public HandRemoveMemberInGroup(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<string>> Handle(KickMemberInGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var myId = ObjectId.Parse(_contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.PrimarySid));
                var groupid = ObjectId.Parse(request.GroupId);
                var memberId = ObjectId.Parse(request.MemberId);

                var myRole = await _unitOfWork.groupRepository.CheckMemberInGroupAsync(groupid, myId);
                var memberRole = await _unitOfWork.groupRepository.CheckMemberInGroupAsync(groupid, memberId);

                if (myRole is null || memberRole is null) return Result<string>.Failuer(GroupError.NotFound);

                if (myRole.Role.Equals(GroupRoles.Member) || myRole.Role.Equals(memberRole.Role) || memberRole.Role.Equals(GroupRoles.Created))
                    return Result<string>.Failuer(GroupError.NotPermission);

                await _unitOfWork.groupRepository.RemoveMemberInGroup(groupid, memberId);

                await _unitOfWork.groupRoomRepository.RemoveGroupRoom(memberId, groupid);

                return Result<string>.Success("Success!");
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
