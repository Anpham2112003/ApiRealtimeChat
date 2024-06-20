using Domain.ResponeModel;
using Domain.ResponeModel.BsonConvert;
using Domain.Ultils;
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

namespace Application.Features.Friend
{
    public class GetWaitListCommand:IRequest<Result<PagingRespone<List<FriendWaitListResponeModel>>>>
    {
        public int index {  get; set; }
        public int limit {  get; set; }

    }

    public class HandGetInforFriendFromWaitListCommand : IRequestHandler<GetWaitListCommand, Result<PagingRespone<List<FriendWaitListResponeModel>>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;

        public HandGetInforFriendFromWaitListCommand(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<PagingRespone<List<FriendWaitListResponeModel>>>> Handle(GetWaitListCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var accountId = _contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.PrimarySid);

                var result = await _unitOfWork.friendRepository.GetInfoFromWatiList(accountId!, request.index, request.limit);

                var page = new PagingRespone<List<FriendWaitListResponeModel>>(request.index, request.limit, result);

                return Result<PagingRespone<List<FriendWaitListResponeModel>>>.Success(page);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
