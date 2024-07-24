using Domain.Entities;
using Domain.Ultils;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Post
{
    public class RemovePostCommand:IRequest<Result<string>>
    {
        public string? PostId {  get; set; }
    }

    public class HandRemovePostCommand : IRequestHandler<RemovePostCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        public HandRemovePostCommand(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<string>> Handle(RemovePostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                
                var AccountId = _contextAccessor.HttpContext!.User.GetIdFromClaim();


                var result =  await _unitOfWork.postRepository.RemovePost(AccountId,request.PostId! );

                if (result.ModifiedCount == 0) return Result<string>.Failuer(new Error("Not found",""));

                await _unitOfWork.commentRepository.RemoveCommentCollection(request.PostId!);

                return Result<string>.Success("Ok!");
            }
            catch (Exception)
            {

                throw;
            }
            
        }
    }
}
