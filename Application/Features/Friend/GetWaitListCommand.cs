using Domain.ResponeModel;
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
    public class GetWaitListCommand:IRequest<Result<ScrollPage<UserResponseModel>>>
    {
        public int index {  get; set; }
        public int limit {  get; set; }

    }

    public class HandGetInforFriendFromWaitListCommand : IRequestHandler<GetWaitListCommand, Result<ScrollPage<UserResponseModel>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;

        public HandGetInforFriendFromWaitListCommand(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<ScrollPage<UserResponseModel>>> Handle(GetWaitListCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var accountId = _contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.PrimarySid);

                var result = await _unitOfWork.friendRepository.GetInfoFromWatiList(accountId!, request.index, request.limit);

                if (result is null || !result.Any()) return Result<ScrollPage<UserResponseModel>>.Failuer(new Error("Not found", "Not Found"));

                var page = new ScrollPage<UserResponseModel>(request.index, request.limit, result);

                return Result<ScrollPage<UserResponseModel>>.Success(page);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
