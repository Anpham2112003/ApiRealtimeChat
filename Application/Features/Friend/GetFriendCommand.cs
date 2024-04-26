
using Application.Errors;
using Application.Ultils;
using Domain.Entites;
using Domain.ResponeModel;
using Domain.ResponeModel.BsonConvert;
using Infrastructure.Unit0fWork;
using MediatR;
using MongoDB.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Friend
{
    public class GetFriendCommand:IRequest<Result<PagingRespone<List<FriendResultConvert>>>>
    {
        public string? AccoutId { get; set; }
        public int skip {  get; set; }
        public int limit { get; set; }

        public GetFriendCommand(string? accoutId, int skip, int limit)
        {
            AccoutId = accoutId;
            this.skip = skip;
            this.limit = limit;
        }
    }

    public class HandGetFriend : IRequestHandler<GetFriendCommand, Result<PagingRespone<List<FriendResultConvert>>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public HandGetFriend(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PagingRespone<List<FriendResultConvert>>>> Handle(GetFriendCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _unitOfWork.friendRepository.GetFriendAysnc(ObjectId.Parse(request.AccoutId), request.skip, request.limit);

                if (result == null) return Result<PagingRespone<List<FriendResultConvert>>>.Failuer(FriendError.DocumentNotFound);

                var page = new PagingRespone<List<FriendResultConvert>>(request.skip, request.limit, result!.Result);

                return Result<PagingRespone<List<FriendResultConvert>>>.Success(page);
            }
            catch (Exception)
            {

                throw;
            }
           
        }
    }
}
