using Domain.Entities;
using Domain.Ultils;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Coment
{
    public class CommentPostCommand:IRequest<Result<Comment>>
    {
        public string? PostId {  get; set; }
        public  string? Content {  get; set; }
        public IFormFile? File { get; set; }
    }

    public class HandCommentPostCommand : IRequestHandler<CommentPostCommand, Result<Comment>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;

        public HandCommentPostCommand(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<Comment>> Handle(CommentPostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var AccountId = _contextAccessor.HttpContext!.User.GetIdFromClaim();

                var filename = GenerateRandomFileName.GenerateFromFile(request.File!);

                if (request.File != null)
                {

                }


                var comment = new Comment
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    AccountId = AccountId,
                    Content = request.Content,
                    ImageUrl = filename,
                    TotalChildComment = 0,
                    CreatedAt = DateTime.UtcNow,
                };

                var result = await _unitOfWork.commentRepository.PushComment(request.PostId!, comment);

                if (result.MatchedCount == 0) return Result<Comment>.Failuer(new Error("Comment", "Comment was block"));

                return Result<Comment>.Success(comment);
            }
            catch (Exception)
            {

                throw;
            };
        }
    }
}
