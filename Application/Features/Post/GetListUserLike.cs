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
    public class GetListUserLike:IRequest<Result<ScrollPage<UserResponseModel>>>
    {
        public string? AccountId {  get; set; }
        public string? PostId {  get; set; }

        public int Skip {  get; set; }
        public int Limit { get; set; }
    }

    public class HandGetListUserLike : IRequestHandler<GetListUserLike, Result<ScrollPage<UserResponseModel>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public HandGetListUserLike(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ScrollPage<UserResponseModel>>> Handle(GetListUserLike request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _unitOfWork.postRepository.GetListUserLikePost(request.AccountId!,  request.PostId!, request.Skip, request.Limit);

                if (!result.Any()) return Result<ScrollPage<UserResponseModel>>.Failuer(new Error("Not Found", ""));

                return Result<ScrollPage<UserResponseModel>>.Success(new ScrollPage<UserResponseModel>
                {
                    Data = result,
                    Index = request.Skip,
                    Limit = request.Limit
                });
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
