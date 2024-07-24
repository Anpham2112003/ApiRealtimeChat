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
    public class RepCommentPostCommand:IRequest<Result<Comment>>
    {
        public string? PostId { get; set; }
        public string? RepId {  get; set; }
        public string? Content { get; set; }
        public IFormFile? File { get; set; }
    }

    public class HandRepCommentPostCommand : IRequestHandler<RepCommentPostCommand, Result<Comment>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;

        public HandRepCommentPostCommand(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<Comment>> Handle(RepCommentPostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var AccountId = _contextAccessor.HttpContext!.User.GetIdFromClaim();

                var comment = new Comment
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Content = request.Content,
                    AccountId = AccountId,
                    ImageUrl = "",
                    ParentId = request.RepId,
                    TotalChildComment = 0,
                    CreatedAt = DateTime.UtcNow,
                };

                var result = await _unitOfWork.commentRepository.RepComment(request.PostId!, request.RepId!, comment);

                if (result.MatchedCount == 0) return Result<Comment>.Failuer(new Error("Comment is block", ""));

                return Result<Comment>.Success(comment);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
