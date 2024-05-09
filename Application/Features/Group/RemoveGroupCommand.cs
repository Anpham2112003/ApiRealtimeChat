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
    public class RemoveGroupCommand:IRequest<Result<string>>
    {
        public string? GroupId { get; set; }

        public RemoveGroupCommand(string? groupId)
        {
            GroupId = groupId;
        }
    }

    public class HandRemoveGroupCommand : IRequestHandler<RemoveGroupCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;

        public HandRemoveGroupCommand(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<string>> Handle(RemoveGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var myId = ObjectId.Parse(_contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.PrimarySid));

                var groupId = ObjectId.Parse(request.GroupId);

                var checkRole = await _unitOfWork.groupRepository.CheckMemberInGroupAsync(groupId, myId);

                if (checkRole is null || !checkRole.Role.Equals(GroupRoles.Created)) return Result<string>.Failuer(GroupError.NotPermission);

                await _unitOfWork.groupRepository.RemoveGroup(groupId);

                return Result<string>.Success("success!");
            }
            catch (Exception)
            {

                throw;
            }
            
        }
    }
}
