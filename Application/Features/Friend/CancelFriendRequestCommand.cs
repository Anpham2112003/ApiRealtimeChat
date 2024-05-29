using Application.Errors;
using Domain.Ultils;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Friend
{
    public class CancelFriendRequestCommand:IRequest<Result<string>>
    {
        public string? Id {  get; set; }
    }

    public class HandCancelFriendRequestCommand : IRequestHandler<CancelFriendRequestCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;

        public HandCancelFriendRequestCommand(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<string>> Handle(CancelFriendRequestCommand request, CancellationToken cancellationToken)
        {
           
            try
            {
                var UserId = _contextAccessor.HttpContext!.User.GetIdFromClaim();

                var result = await _unitOfWork.friendRepository.CancelFriendResquestAsync(UserId, request.Id!);

                if(result.MatchedCount==0) return Result<string>.Failuer(FriendError.NotFoundInWaitList);

                return Result<string>.Success("Success!");
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
