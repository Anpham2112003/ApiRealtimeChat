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
    public class ChangeRolesInGroupCommand:IRequest<Result<string>>
    {
        public string? Id {  get; set; }
        public string? GroupId { get; set; }
        public GroupRoles roles { get; set; }
    }
    public class HandChangeRolesInGroupCommand : IRequestHandler<ChangeRolesInGroupCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        public HandChangeRolesInGroupCommand(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<string>> Handle(ChangeRolesInGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var Id = ObjectId.Parse(request.Id);

                var myId = ObjectId.Parse(_contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.PrimarySid));

                var groupId = ObjectId.Parse(request.GroupId);

                var yourAccount = await _unitOfWork.groupRepository.CheckMemberInGroupAsync(groupId, myId);

                if (yourAccount == null) return Result<string>.Failuer(GroupError.NotFound);

                if (!yourAccount.Role.Equals(GroupRoles.Created) || request.roles.Equals(GroupRoles.Created)) return Result<string>.Failuer(GroupError.NotPermission);

                await _unitOfWork.groupRepository.UpdateRoleInGroup(groupId,Id, request.roles);

                return Result<string>.Success("Success!");
            }
            catch (Exception)
            {

                throw;
            }
           

            
        }
    }
}
