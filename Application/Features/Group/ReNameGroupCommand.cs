using Application.Errors;
using Application.Ultils;
using Domain.Enums;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Group
{
    public class ReNameGroupCommand:IRequest<Result<string>>
    {
        public string? GroupId { get; set; } 
        public string? Name { get; set; }
    }

    public class HandRenameGroupCommand : IRequestHandler<ReNameGroupCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HandRenameGroupCommand(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<string>> Handle(ReNameGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.PrimarySid);

                var groupId = ObjectId.Parse(request.GroupId);

                var member = await _unitOfWork.groupRepository.CheckMemberInGroupAsync(groupId, ObjectId.Parse(userId));

                if (member == null) return Result<string>.Failuer(GroupError.NotFound);

                if (member.Role.Equals(GroupRoles.Admin)) return Result<string>.Failuer(GroupError.NotPermission);

                await _unitOfWork.groupRepository.RenameGroup(groupId, request.Name!);

                return Result<string>.Success("success!");
            }
            catch (Exception)
            {

                throw;
            }
            
        }
    }
}
