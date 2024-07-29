using Domain.ResponeModel;
using Domain.Ultils;
using Infrastructure.Unit0fWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Coment
{
    public class GetRepComment:IRequest<Result<ScrollPage<CommentResponseModel>>>
    {
        public string? AccountId {  get; set; }
        public string? PostId {  get; set; }
        public string? ParentId {  get; set; }
        public int skip {  get; set; }
        public int limit {  get; set; }

        public GetRepComment(string? accountId, string? postId, string? parentId, int skip, int limit)
        {
            AccountId = accountId;
            PostId = postId;
            ParentId = parentId;
            this.skip = skip;
            this.limit = limit;
        }
    }

    public class HandGetRepComment : IRequestHandler<GetRepComment, Result<ScrollPage<CommentResponseModel>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public HandGetRepComment(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ScrollPage<CommentResponseModel>>> Handle(GetRepComment request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _unitOfWork.commentRepository.GetRepComment(request.AccountId!, request.PostId!, request.ParentId!, request.skip, request.limit);

                if (result.Any()) return Result<ScrollPage<CommentResponseModel>>.Success(new ScrollPage<CommentResponseModel>
                {
                    Index = request.skip,
                    Limit = request.limit,
                    Data = result,
                });

                return Result<ScrollPage<CommentResponseModel>>.Failuer(new Error("Notfound", "NotFound"));
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
