using Domain.Errors;
using Domain.ResponeModel.BsonConvert;
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
    public class GetMemberInGroupCommand:IRequest<Result<List<MembersGroupConvert>>>
    {
        public string? Id { get; set; }
        public int Skip {  get; set; }
        public int Limit {  get; set; }
    }
    public class HandGetMemberInGroupCommand : IRequestHandler<GetMemberInGroupCommand, Result<List<MembersGroupConvert>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public HandGetMemberInGroupCommand(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<MembersGroupConvert>>> Handle(GetMemberInGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _unitOfWork.groupRepository.GetMembersInGroup(request.Id!, request.Skip, request.Limit);

                if(result.Any()) return Result<List<MembersGroupConvert>>.Success(result);

                return Result<List<MembersGroupConvert>>.Failuer(GroupError.GroupNotFound);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
