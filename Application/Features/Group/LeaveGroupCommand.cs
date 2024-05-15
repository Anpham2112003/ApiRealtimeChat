using Application.Ultils;
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
    public class LeaveGroupCommand:IRequest<Result<string>>
    {
        public string? Id {  get; set; }

        public LeaveGroupCommand(string? id)
        {
            Id = id;
        }
    }

    public class HandLeaveGroupCommand : IRequestHandler<LeaveGroupCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;

        public HandLeaveGroupCommand(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<string>> Handle(LeaveGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = ObjectId.Parse(_contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.PrimarySid));

                var groupId = ObjectId.Parse(request.Id);

                await _unitOfWork.groupRepository.RemoveMemberInGroup(groupId, userId);

                await _unitOfWork.groupRoomRepository.RemoveGroupRoom( userId,groupId);

                return Result<string>.Success(request.Id);
            }
            catch (Exception)
            {

                throw;
            }
           
        }
    }
}
