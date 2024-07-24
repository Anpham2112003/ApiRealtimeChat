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
    public class UnLikePostCommand:IRequest<Result<bool>>
    {
        public string? AccountId {  get; set; }
        public string? PostId {  get; set; }
    }

    public class HandUnlikePostCommand : IRequestHandler<UnLikePostCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        public HandUnlikePostCommand(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<bool>> Handle(UnLikePostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var AccountId = _contextAccessor.HttpContext!.User.GetIdFromClaim();

                var result = await _unitOfWork.postRepository.UnLikePost(AccountId, request.AccountId!, request.PostId!);

                if (result.ModifiedCount == 0) return Result<bool>.Failuer(new Error("Not found", ""));

                return Result<bool>.Success();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
