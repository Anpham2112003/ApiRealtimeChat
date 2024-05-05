using Application.Ultils;
using Domain.ResponeModel;
using Domain.ResponeModel.BsonConvert;
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
    public class GetInforFriendFromWaitListCommand:IRequest<Result<PagingRespone<GetInfoWaitAccecptConvert>>>
    {
        public int index {  get; set; }
        public int limit {  get; set; }

    }

    public class HandGetInforFriendFromWaitListCommand : IRequestHandler<GetInforFriendFromWaitListCommand, Result<PagingRespone<GetInfoWaitAccecptConvert>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;

        public HandGetInforFriendFromWaitListCommand(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<PagingRespone<GetInfoWaitAccecptConvert>>> Handle(GetInforFriendFromWaitListCommand request, CancellationToken cancellationToken)
        {
            var accountId = _contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.PrimarySid);

            var result  = await _unitOfWork.friendRepository.GetInfoFromWatiList(ObjectId.Parse(accountId),request.index,request.limit);

            var page = new PagingRespone<GetInfoWaitAccecptConvert>(request.index, request.limit, result);

            return Result<PagingRespone<GetInfoWaitAccecptConvert>>.Success(page);
        }
    }
}
