using Application.Errors;
using Application.Ultils;
using Domain.Entites;
using Infrastructure.Unit0fWork;
using MediatR;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Group
{
    public class AddMemberToGroup:IRequest<Result<string>>
    {
        public string? GroupId { get; set; }
        public string? UserId { get; set; }
    }

    public class HandAddMemberToGroup : IRequestHandler<AddMemberToGroup, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public HandAddMemberToGroup(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(AddMemberToGroup request, CancellationToken cancellationToken)
        {
            try
            {
                var groupId=ObjectId.Parse(request.GroupId);

                var memberId = ObjectId.Parse(request.UserId);

                var checkMember = await _unitOfWork.groupRepository.CheckMemberInGroupAsync(groupId, memberId);

                if (checkMember is not null) return Result<string>.Failuer(GroupError.MemberExist);

                var member = new Member(memberId, "default", Domain.Enums.GroupRoles.Member);

;               await _unitOfWork.groupRepository.AddMemberToGroup(groupId,memberId,member);

                return Result<string>.Success(request.GroupId);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
