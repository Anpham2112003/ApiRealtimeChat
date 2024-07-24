using Domain.ResponeModel;
using Domain.Ultils;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Post
{
    public class GetNewPosts:IRequest<Result<ScrollPage<PostResponseModel>>>
    {
        public int skip { get; set; }
        public int take { get; set; }

        public GetNewPosts(int skip, int take)
        {
            this.skip = skip;
            this.take = take;
        }
    }
    public class HandGetNewPost : IRequestHandler<GetNewPosts, Result<ScrollPage<PostResponseModel>>>
{
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;

        public HandGetNewPost(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<ScrollPage<PostResponseModel>>> Handle(GetNewPosts request, CancellationToken cancellationToken)
        {
            try
            {
                var AccountId = _contextAccessor.HttpContext!.User.GetIdFromClaim();

                var result =  await _unitOfWork.postRepository.GetLastPostFromListFriend(AccountId, request.skip, request.take);

                if (result.Any()) return Result<ScrollPage<PostResponseModel>>.Success(new ScrollPage<PostResponseModel>
                {
                    Data = result,
                    Index = request.skip,
                    Limit=request.take
                });

                return Result<ScrollPage<PostResponseModel>>.Failuer(new Error("Not Found", ""));
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
