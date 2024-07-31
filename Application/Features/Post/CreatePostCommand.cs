using Domain.Entities;
using Domain.Ultils;
using Infrastructure.Services.FileService;
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
        public bool AllowComment {  get; set; }
    }

    public class HandCreatePostCommand : IRequestHandler<CreatePostCommand, Result<Domain.Entities.Post>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IFileService _fileService;

        public HandCreatePostCommand(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
            _fileService = fileService;
        }

        public async Task<Result<Domain.Entities.Post>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var AccountId = _contextAccessor.HttpContext!.User.GetIdFromClaim();

                var fileNames = new List<string> { };

                if ( request.Files !=null && request.Files!.Any())
                {

                    foreach (var item in request.Files!)
                    {
                        var filename = ObjectId.GenerateNewId() + Path.GetExtension(item.FileName);

                        var storePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Images");

                        await _fileService.WriteFileAsync(storePath, filename, item);

                        fileNames.Add(Path.Combine("Uploads", "Images", filename));
                    }
                }

                var Post = new Domain.Entities.Post
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Content = request.Content,
                    AccountId = AccountId,
                    Images = fileNames,
                    Likes = 0,
                    AllowComment = request.AllowComment,
                    TotalComment = 0,
                    HiddenComment = false,
                    Comments = new List<Comment> { },
                    ListLike = new List<ObjectId> { },
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };

                await _unitOfWork.postRepository.InsertPost(AccountId, Post);

                return Result<Domain.Entities.Post>.Success(Post);
            }
            catch (Exception )
            {
                
                throw;
            }
        }
    }
}
