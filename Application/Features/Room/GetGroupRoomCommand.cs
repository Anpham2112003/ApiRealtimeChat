using Application.Errors;
using Application.Ultils;
using Domain.ResponeModel;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.GroupRoom
{
    public class GetGroupRoomCommand:IRequest<Result<Object>>
    {
        public int skip {  get; set; }
        public int limit { get; set; }

        public GetGroupRoomCommand(int skip, int limit)
        {
            this.skip = skip;
            this.limit = limit;
        }
    }

    public class HandGetGroupRoomCommand : IRequestHandler<GetGroupRoomCommand, Result<Object>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;

        public HandGetGroupRoomCommand(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<object>> Handle(GetGroupRoomCommand request, CancellationToken cancellationToken)
        {
            var userId = _contextAccessor.HttpContext!.User.GetIdFromClaim();

            var groups = await _unitOfWork.groupRoomRepository.GetGroupAsync(userId, request.skip, request.limit);

            if (groups == null) return Result<Object>.Failuer(RoomError.NotFound);

            return Result<Object>.Success(new
            {
                skip=request.skip,
                limit=request.limit,
                totalRoom=groups.TotalGroup,
                Data=groups.Groups
                
            });;
        }
    }
}
