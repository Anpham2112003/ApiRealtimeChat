﻿using Domain.Entities;
using Domain.ResponeModel;
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
    public class GetCommentPost:IRequest<Result<ScrollPage<CommentResponseModel>>>
    {
        public string? AccountId {  get; set; }
        public string? PostId {  get; set; }
        public int skip {  get; set; }
        public int limit {  get; set; }

        public GetCommentPost(string? accountId, string? postId, int skip, int limit)
        {
            AccountId = accountId;
            PostId = postId;
            this.skip = skip;
            this.limit = limit;
        }
    }
    public class HandGetCommentPost : IRequestHandler<GetCommentPost, Result<ScrollPage<CommentResponseModel>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public HandGetCommentPost(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result< ScrollPage<CommentResponseModel>>> Handle(GetCommentPost request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _unitOfWork.commentRepository.GetCommentPost(request.AccountId!, request.PostId!, request.skip, request.limit);

                if (result.Any()) return Result<ScrollPage<CommentResponseModel>>.Success(new ScrollPage<CommentResponseModel>
                {
                    Index = request.skip,
                    Limit = request.limit,
                    Data = result,
                });

                return Result<ScrollPage<CommentResponseModel>>.Failuer(new Error("NotFound","NotFound"));
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
