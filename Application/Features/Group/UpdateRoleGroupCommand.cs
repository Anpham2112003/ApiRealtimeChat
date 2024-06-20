using Domain.Enums;
using Domain.Ultils;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Group
{
    public class UpdateRoleGroupCommand:IRequest<Result<string>>
    {
        public string? GroupId { get; set; }
        public string? MemberId {  get; set; }
        public GroupRoles Role { get; set; }
    }

    public class HandUpdateRoleInGroup : IRequestHandler<UpdateRoleGroupCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;

        public HandUpdateRoleInGroup(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<string>> Handle(UpdateRoleGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _contextAccessor.HttpContext!.User.GetIdFromClaim();

                var getRoleUser = await _unitOfWork.groupRepository.GetMemberInGroup(request.GroupId!, userId);

                if (getRoleUser is null || !getRoleUser.Role.Equals(GroupRoles.Created)) 
                    return Result<string>.Failuer(new Error("Not Permisstion", ""));

                if(request.Role.Equals(GroupRoles.Created))
                {
                    await _unitOfWork.groupRepository.UpdateRole(request.GroupId!, userId, GroupRoles.Member);

                    await _unitOfWork.groupRepository.UpdateRole(request.GroupId!, request.MemberId!, GroupRoles.Created);

                    return Result<string>.Success("Ok!");
                }

              
                 await _unitOfWork.groupRepository.UpdateRole(request.GroupId!, request.MemberId!, request.Role);

                return Result<string>.Success("Ok!");
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
