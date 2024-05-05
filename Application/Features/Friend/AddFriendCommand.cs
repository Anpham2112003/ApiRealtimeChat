using Application.Errors;
using Application.Ultils;
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
    public class AddFriendCommand:IRequest<Result<Object>>
    {
        
        public string? FriendId { get; set; }
    }

    public class HandAddFriendCommand : IRequestHandler<AddFriendCommand, Result<Object>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;

        public HandAddFriendCommand(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<object>> Handle(AddFriendCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var accountId = _contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.PrimarySid);

                var friend = await _unitOfWork.accountRepository.CheckAccountExist(ObjectId.Parse(request.FriendId));

                if ( friend is false) return Result<Object>.Failuer(FriendError.DocumentNotFound);

                var T1 = _unitOfWork.friendRepository.AcceptFriend(ObjectId.Parse(accountId), ObjectId.Parse(request.FriendId));

                var T2 = _unitOfWork.friendRepository.AddFriendAsync(ObjectId.Parse(request.FriendId), ObjectId.Parse(accountId));

                await Task.WhenAll(T1, T2);

                return Result<Object>.Success();
            }
            catch (Exception)
            {

                throw;
            }
            
        }
    }
}
