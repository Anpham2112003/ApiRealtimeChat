using Domain.Errors;
using Domain.ResponeModel;
using Domain.Ultils;
using Infrastructure.Unit0fWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Group
{
    public class GetMemberInGroupCommand:IRequest<Result<IEnumerable<MembersGroupResponseModel>>>
    {
        public string? Id { get; set; }
        public int Skip {  get; set; }
        public int Limit {  get; set; }
    }
    public class HandGetMemberInGroupCommand : IRequestHandler<GetMemberInGroupCommand, Result<IEnumerable<MembersGroupResponseModel>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public HandGetMemberInGroupCommand(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IEnumerable<MembersGroupResponseModel>>> Handle(GetMemberInGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _unitOfWork.groupRepository.GetMembersInGroup(request.Id!, request.Skip, request.Limit);

                if(result.Any()) return Result<IEnumerable<MembersGroupResponseModel>>.Success(result);

                return Result<IEnumerable<MembersGroupResponseModel>>.Failuer(GroupError.GroupNotFound);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
