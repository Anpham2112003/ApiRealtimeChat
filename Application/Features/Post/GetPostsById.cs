using Domain.ResponeModel;
using Domain.Ultils;
using Infrastructure.Unit0fWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Post
{
    public class GetPostsById:IRequest<Result<ScrollPage<PostResponseModel>>>
    {
        public string? AccountId {  get; set; }
        public int skip {  get; set; }
        public int limit { get; set; }

        public GetPostsById(string? accountId, int skip, int limit)
        {
            AccountId = accountId;
            this.skip = skip;
            this.limit = limit;
        }
    }

    public class HandGetPostsById : IRequestHandler<GetPostsById, Result<ScrollPage<PostResponseModel>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public HandGetPostsById(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ScrollPage<PostResponseModel>>> Handle(GetPostsById request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _unitOfWork.postRepository.GetPostById(request.AccountId!, request.skip, request.limit);

                if (result.Any()) return Result<ScrollPage<PostResponseModel>>.Success(new ScrollPage<PostResponseModel>
                {
                    Data = result,
                    Index = request.skip,
                    Limit = request.limit
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
