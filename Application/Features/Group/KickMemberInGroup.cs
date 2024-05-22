using Domain.Enums;
using Domain.Errors;
using Domain.Ultils;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Http;
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
        public HandKickMemberInGroup(IUnitOfWork unitOfWork, IHttpContextAccessor accessor)
        {
            _unitOfWork = unitOfWork;
            _accessor = accessor;
        }

        public async Task<Result<string>> Handle(KickMemberInGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var User = await _unitOfWork.groupRepository
                    .GetMemberInGroup(request.Id!, _accessor.HttpContext!.User.FindFirstValue(ClaimTypes.PrimarySid));

                var Member = await _unitOfWork.groupRepository.GetMemberInGroup(request.Id!, request.MemberId!);

                if (User is null || Member is null) return Result<string>.Failuer(GroupError.UserNotFound);


                if (User.Role.Equals(GroupRoles.Admin) && Member.Role.Equals(GroupRoles.Member) || User.Role.Equals(GroupRoles.Created))
                {
                    await _unitOfWork.groupRepository.KickMemberInGroup(request.Id!, request.MemberId!);

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
