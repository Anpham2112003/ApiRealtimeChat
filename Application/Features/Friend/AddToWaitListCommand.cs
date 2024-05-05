using Application.Errors;
using Application.Ultils;
using Domain.Entites;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Friend
{
    public class AddToWaitListCommand:IRequest<Result<string>>
    {
        public string? FriendAccountId { get; set; }
    }

    public class HandAddToWaitListCommand : IRequestHandler<AddToWaitListCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;

        public HandAddToWaitListCommand(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<string>> Handle(AddToWaitListCommand request, CancellationToken cancellationToken)
        {
            try
            {
               
                var accountId = _contextAccessor.HttpContext!.Request.HttpContext.User.FindFirstValue(ClaimTypes.PrimarySid);

                var friendId = ObjectId.Parse(request.FriendAccountId);

                var checkFriendAccount = await _unitOfWork.accountRepository.CheckAccountExist(ObjectId.Parse(request.FriendAccountId));

                if ( !checkFriendAccount)

                return Result<string>.Failuer(FriendError.DocumentNotFound);

                await _unitOfWork.friendRepository.AddToWaitlistAsync(ObjectId.Parse(accountId),friendId);

                return Result<string>.Success("Success!");
            }
            catch (Exception)
            {

                throw;
            }
            
        }
    }
}
