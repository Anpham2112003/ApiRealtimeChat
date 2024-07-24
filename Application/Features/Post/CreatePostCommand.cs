using Domain.Entities;
using Domain.Ultils;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Post
{
    public class CreatePostCommand:IRequest<Result<Domain.Entities.Post>>
    {
        public string? Content {  get; set; }
        public List<IFormFile>? Files { get; set; }
    }

    public class HandCreatePostCommand : IRequestHandler<CreatePostCommand, Result<Domain.Entities.Post>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;

        public HandCreatePostCommand(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<Domain.Entities.Post>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var AccountId = _contextAccessor.HttpContext!.User.GetIdFromClaim();


                var fileNames = GenerateRandomFileName.GenerateFromFiles(request.Files!).ToList();

                var Post = new Domain.Entities.Post
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Content = request.Content,
                    AccountId=AccountId,
                    Images = fileNames,
                    Likes = 0,
                    ListLike = new List<ObjectId> { },
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };

                var commentCollection = new CommentCollection
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    PostId = Post.Id,
                    AllowComment = true,
                    CreatedAt = DateTime.UtcNow,
                    Comments = new List<Comment> { },
                    TotalComment = 0,
                    UpdatedAt = DateTime.UtcNow,
                };

                await _unitOfWork.postRepository.InsertPost(AccountId, Post);

                await _unitOfWork.commentRepository.InsertAsync(commentCollection);
                

                return Result<Domain.Entities.Post>.Success(Post);
            }
            catch (Exception )
            {
                
                throw;
            }
        }
    }
}
