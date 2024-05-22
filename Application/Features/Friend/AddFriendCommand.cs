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

                var friend = await _unitOfWork.accountRepository.CheckAccountExist(request.FriendId!);

                if ( friend is false) return Result<Object>.Failuer(FriendError.DocumentNotFound);

                await _unitOfWork.friendRepository.AcceptFriend(accountId!, request.FriendId!);

                await _unitOfWork.friendRepository.AddFriendAsync(request.FriendId!, accountId!);

             
                return Result<Object>.Success();
            }
            catch (Exception)
            {

                throw;
            }
            
        }
    }
}
