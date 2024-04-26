using Application.Errors;
using Application.Ultils;
using Infrastructure.Unit0fWork;
using MediatR;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Friend
{
    public class RemoveFriendCommand:IRequest<Result<ObjectId>>
    {
        public string? AccountId { get;  set; }
        public string? FriendId { get; set; }

        public RemoveFriendCommand(string? accountId, string? friendId)
        {
            AccountId = accountId;
            FriendId = friendId;
        }
    }


    public class HandRemoveFriendCommand : IRequestHandler<RemoveFriendCommand, Result<ObjectId>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public HandRemoveFriendCommand(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        async Task<Result<ObjectId>> IRequestHandler<RemoveFriendCommand, Result<ObjectId>>.Handle(RemoveFriendCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var account = await _unitOfWork.accountRepository.CheckAccountExist(ObjectId.Parse(request.AccountId));

                var friend = await _unitOfWork.accountRepository.CheckAccountExist(ObjectId.Parse(request.FriendId));

                if (!account || !friend) return Result<ObjectId>.Failuer(FriendError.DocumentNotFound);

                var t1 =  _unitOfWork.friendRepository
                    .RemoveFriendAsync(ObjectId.Parse(request.AccountId),ObjectId.Parse(request.FriendId));

                var t2 = _unitOfWork.friendRepository
                    .RemoveFriendAsync(ObjectId.Parse(request.FriendId),ObjectId.Parse(request.AccountId));

                  await Task.WhenAll(t1, t2);

                return Result<ObjectId>.Success(ObjectId.Parse(request.FriendId));


            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
