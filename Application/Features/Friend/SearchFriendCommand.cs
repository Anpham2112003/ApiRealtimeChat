using Application.Errors;
using Domain.Entities;
using Domain.ResponeModel;
using Domain.ResponeModel.BsonConvert;
using Domain.Ultils;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Friend
{
    public class SearchFriendCommand:IRequest<Result<List<SearchFriendResponeModel>>>
    {
        public string? Name { get; set; }
        
    }

    public class HandSearchFriendCommand : IRequestHandler<SearchFriendCommand, Result<List<SearchFriendResponeModel>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        public HandSearchFriendCommand(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<List<SearchFriendResponeModel>>> Handle(SearchFriendCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var UserId = _contextAccessor.HttpContext!.User.GetIdFromClaim();

                var result = await _unitOfWork.friendRepository.FriendSearchAsync(request.Name!, UserId);

                if (result.Any()) return Result<List<SearchFriendResponeModel>>.Success(result); 

                return Result<List<SearchFriendResponeModel>>.Failuer(FriendError.DocumentNotFound);
            }
            catch (Exception)
            {

                throw;
            }
           
        }
    }
}
