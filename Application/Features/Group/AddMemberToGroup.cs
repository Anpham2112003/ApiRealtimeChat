using Application.Ultils;
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
                await _unitOfWork.groupRepository.AddMemberToGroup(ObjectId.Parse(request.GroupId),ObjectId.Parse(request.UserId));

                return Result<string>.Success(request.GroupId);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
