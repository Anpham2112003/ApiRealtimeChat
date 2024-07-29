using Amazon.Runtime.Internal;
using Domain.Ultils;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Coment
{
    public class DeleteCommentCommand:IRequest<Result<string>>
    {
        public string? AccoutId { get; set; }
        public string? PostId {  get; set; }
        public string? CommentId {  get; set; }
        public string? ParentId {  get; set; }
    }

    public class HandDeleteCommentCommand : IRequestHandler<DeleteCommentCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;

        public HandDeleteCommentCommand(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<string>> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var MyId = _contextAccessor.HttpContext!.User.GetIdFromClaim();

                var result = await _unitOfWork.commentRepository.RemoveComment(request.AccoutId!, MyId,request.PostId!,request.CommentId!,request.ParentId!);

                if (result.ModifiedCount == 0 ) return Result<string>.Failuer(new Error("NOT", ""));

                return Result<string>.Success("Ok!");
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
