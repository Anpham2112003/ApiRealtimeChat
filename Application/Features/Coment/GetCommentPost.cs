using Domain.Entities;
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
    public class GetCommentPost:IRequest<Result<CommentCollection>>
    {
        public int PostId {  get; set; }
        public int skip {  get; set; }
        public int limit {  get; set; }

    }
    public class HandGetCommentPost : IRequestHandler<GetCommentPost, Result<CommentCollection>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public HandGetCommentPost(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<Result<CommentCollection>> Handle(GetCommentPost request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
