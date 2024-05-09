using Application.Errors;
using Application.Ultils;
using Domain.Entites;
using Domain.ResponeModel;
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
    public class GetMemberInGroupCommand:IRequest<Result<PagingRespone<List<Member>>>>
    {
        public string? GroupId {  get; set; }
        public int skip {  get; set; }
        public int limit { get; set; }

        public GetMemberInGroupCommand(string? groupId, int skip, int limit)
        {
            GroupId = groupId;
            this.skip = skip;
            this.limit = limit;
        }
    }
    public class HandGetMemberInGroup : IRequestHandler<GetMemberInGroupCommand, Result<PagingRespone<List<Member>>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public HandGetMemberInGroup(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PagingRespone<List<Member>>>> Handle(GetMemberInGroupCommand request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.groupRepository
                .GetMembersInGroupAsync(ObjectId.Parse(request.GroupId),request.skip,request.limit);

            if (!result.Any()) return Result<PagingRespone<List<Member>>>.Failuer(GroupError.MemberNotFound);

            var page = new PagingRespone<List<Member>>(request.skip, request.limit,result);

            return Result<PagingRespone<List<Member>>>.Success(page);   
        }
    }
}
