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
    public class RejectFriendWaitListCommand:IRequest<Result<string>>
    {
        public string? RejectId {  get; set; }

        public RejectFriendWaitListCommand(string? rejectId)
        {
            RejectId = rejectId;
        }
    }

    public class HandRejectFriendWaitListCommand : IRequestHandler<RejectFriendWaitListCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContext;

        public HandRejectFriendWaitListCommand(IUnitOfWork unitOfWork, IHttpContextAccessor httpContext)
        {
            _unitOfWork = unitOfWork;
            _httpContext = httpContext;
        }

        public async Task<Result<string>> Handle(RejectFriendWaitListCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var accountId = _httpContext.HttpContext!.User.FindFirstValue(ClaimTypes.PrimarySid);

                var filter = Builders<FriendCollection>
                    .Filter
                    .Eq(x => x.AccountId, accountId);

                var update = Builders<FriendCollection>
                    .Update
                    .Pull(x => x.WaitingList, ObjectId.Parse(request.RejectId));

                var result = await _unitOfWork.friendRepository.UpdateAsync(filter, update);

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
