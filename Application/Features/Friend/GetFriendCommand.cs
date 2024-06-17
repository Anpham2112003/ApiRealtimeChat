
using Application.Errors;
using Domain.ResponeModel;
using Domain.Ultils;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Friend
{
    public class GetFriendCommand:IRequest<Result<PagingRespone<List<UserConvert>>>>
    {
       
        public int skip {  get; set; }
        public int limit { get; set; }

        public GetFriendCommand( int skip, int limit)
        {
           
            this.skip = skip;
            this.limit = limit;
        }
    }

    public class HandGetFriend : IRequestHandler<GetFriendCommand, Result<PagingRespone<List<UserConvert>>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        public HandGetFriend(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<PagingRespone<List<UserConvert>>>> Handle(GetFriendCommand request, CancellationToken cancellationToken)
        {
            try {

                var UserId = _contextAccessor.HttpContext!.User.GetIdFromClaim(); 

                var result = await _unitOfWork.friendRepository.GetFriendAysnc(UserId, request.skip, request.limit);

                if (!result.Any()) return Result<PagingRespone<List<UserConvert>>>.Failuer(FriendError.DocumentNotFound);

                var page = new PagingRespone<List<UserConvert>>(request.skip, request.limit, result);

                return Result<PagingRespone<List<UserConvert>>>.Success(page);
            }
            catch (Exception)
            {

                throw;
            }
           
        }
    }
}
