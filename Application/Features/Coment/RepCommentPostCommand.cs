using Domain.Entities;
using Domain.Ultils;
using Infrastructure.Services.FileService;
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
        public string? AccountId { get; set; }
        public string? PostId { get; set; }
        public string? RepId {  get; set; }
        public string? Content { get; set; }
        public IFormFile? File { get; set; }
    }

    public class HandRepCommentPostCommand : IRequestHandler<RepCommentPostCommand, Result<Comment>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IFileService _fileService;
        public HandRepCommentPostCommand(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
            _fileService = fileService;
        }

        public async Task<Result<Comment>> Handle(RepCommentPostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var AccountId = _contextAccessor.HttpContext!.User.GetIdFromClaim();

                var filename = ObjectId.GenerateNewId() + Path.GetExtension(request.File!.FileName);

                var storePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Images");

                var imagepath = new StringBuilder();

                if (request.File != null)
                {
                    imagepath.Append(Path.Combine("Uploads", "Images", filename));

                    await _fileService.WriteFileAsync(storePath, filename, request.File);
                }

                var comment = new Comment
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Content = request.Content,
                    AccountId = AccountId,
                    PostId = request.PostId,
                    ImageUrl = imagepath.ToString(),
                    ParentId = request.RepId,
                    TotalChildComment = 0,
                    CreatedAt = DateTime.UtcNow,
                };

                var result = await _unitOfWork.commentRepository.RepComment(request.AccountId!,request.PostId!, request.RepId!, comment);

                if (result.MatchedCount == 0)
                {
                    _fileService.RemoveFile(Path.Combine(storePath, filename));

                    return Result<Comment>.Failuer(new Error("Comment is block", ""));
                }

                return Result<Comment>.Success(comment);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
