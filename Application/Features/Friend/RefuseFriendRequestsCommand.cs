using Application.Errors;
using Domain.Entities;
using Domain.Ultils;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Friend
{
    public class RefuseFriendRequestsCommand:IRequest<Result<string>>
    {
        public string? Id {  get; set; }

        public RefuseFriendRequestsCommand(string? rejectId)
        {
            Id = rejectId;
        }
    }

    public class HandRejectFriendWaitListCommand : IRequestHandler<RefuseFriendRequestsCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContext;

        public HandRejectFriendWaitListCommand(IUnitOfWork unitOfWork, IHttpContextAccessor httpContext)
        {
            _unitOfWork = unitOfWork;
            _httpContext = httpContext;
        }

        public async Task<Result<string>> Handle(RefuseFriendRequestsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var accountId = _httpContext.HttpContext!.User.FindFirstValue(ClaimTypes.PrimarySid);
                
                var result = await _unitOfWork.friendRepository.RejectFriendRequest(accountId,request.Id!);

                if (result.ModifiedCount == 0) return Result<string>.Failuer(FriendError.DocumentNotFound);

                return Result<string>.Success("success!");
            }
            catch (Exception)
            {

                throw;
            }
            
        }
    }
}
