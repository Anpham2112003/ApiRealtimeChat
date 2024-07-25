﻿using Domain.Ultils;
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
    public class UnHiddenCommentCommand:IRequest<Result<string>>
    {
        public string? PostId {  get; set; }
    }

    public class HandUnHiddenCommentCommand : IRequestHandler<UnHiddenCommentCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;

        public HandUnHiddenCommentCommand(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<string>> Handle(UnHiddenCommentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var AccountId = _contextAccessor.HttpContext!.User.GetIdFromClaim();

                var result = await _unitOfWork.commentRepository.UnHiddenComment(AccountId, request.PostId!);

                if (result.ModifiedCount == 0) return Result<string>.Failuer(new Error("", ""));

                return Result<string>.Success("Ok!");

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
