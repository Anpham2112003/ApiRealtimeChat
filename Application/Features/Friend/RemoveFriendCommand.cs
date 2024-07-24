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
                var myId = _contextAccessor.HttpContext?.User.GetIdFromClaim();

                var hasFriend = await _unitOfWork.friendRepository.HasFriend(myId!, request.FriendId!);

                if ( !hasFriend) return Result<ObjectId>.Failuer(new Error("Friend","Has not Friend"));

                await  _unitOfWork.friendRepository.RemoveFriendAsync(myId!,request.FriendId!);

                return Result<ObjectId>.Success(ObjectId.Parse(request.FriendId));


            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
