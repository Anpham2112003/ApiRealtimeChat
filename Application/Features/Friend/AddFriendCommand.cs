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
    public class AddFriendCommand:IRequest<Result<Object>>
    {
        public string? AccountId {  get; set; }
        public string? FriendId { get; set; }
    }

    public class HandAddFriendCommand : IRequestHandler<AddFriendCommand, Result<Object>>
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public HandAddFriendCommand(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<object>> Handle(AddFriendCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var account =await _unitOfWork.accountRepository.CheckAccountExist(ObjectId.Parse(request.AccountId));

                var friend = await _unitOfWork.accountRepository.CheckAccountExist(ObjectId.Parse(request.FriendId));

                if (account is false || friend is false) return Result<Object>.Failuer(FriendError.DocumentNotFound);

                var T1 = _unitOfWork.friendRepository.AddFriendAsync(ObjectId.Parse(request.AccountId), ObjectId.Parse(request.FriendId));

                var T2 = _unitOfWork.friendRepository.AddFriendAsync(ObjectId.Parse(request.FriendId), ObjectId.Parse(request.AccountId));

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
