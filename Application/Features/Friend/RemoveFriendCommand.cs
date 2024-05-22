using Application.Errors;
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
    public class RemoveFriendCommand:IRequest<Result<ObjectId>>
    {
        
        public string? FriendId { get; set; }

        public RemoveFriendCommand(string? friendId)
        {
            FriendId = friendId;
        }
    }


    public class HandRemoveFriendCommand : IRequestHandler<RemoveFriendCommand, Result<ObjectId>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;

        public HandRemoveFriendCommand(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        async Task<Result<ObjectId>> IRequestHandler<RemoveFriendCommand, Result<ObjectId>>.Handle(RemoveFriendCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var accountId = _contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.PrimarySid);

                var friend = await _unitOfWork.accountRepository.CheckAccountExist(request.FriendId!);

                if ( !friend) return Result<ObjectId>.Failuer(FriendError.DocumentNotFound);

                await  _unitOfWork.friendRepository
                    .RemoveFriendAsync(accountId!,request.FriendId!);

                await _unitOfWork.friendRepository
                    .RemoveFriendAsync(request.FriendId!,accountId!);


                return Result<ObjectId>.Success(ObjectId.Parse(request.FriendId));


            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
