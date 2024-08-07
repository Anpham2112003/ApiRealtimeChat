﻿using Domain.Entities;
using Domain.ResponeModel;
using Domain.Ultils;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Friend
{
    public class GetFriendNotInGroupCommand:IRequest<Result<ScrollPage<UserResponseModel>>>
    {
        public string Id {  get; set; }
        public int skip {  get; set; }
        public int take { get; set; }

        public GetFriendNotInGroupCommand(string id, int skip, int take)
        {
            Id = id;
            this.skip = skip;
            this.take = take;
        }
    }
    public class HandGetFriendNotInGroupCommand : IRequestHandler<GetFriendNotInGroupCommand, Result<ScrollPage<UserResponseModel>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;

        public HandGetFriendNotInGroupCommand(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<ScrollPage<UserResponseModel>>> Handle(GetFriendNotInGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var UserId = _contextAccessor.HttpContext!.User.GetIdFromClaim();

                var result = await _unitOfWork.friendRepository.GetFriendNotInGroup(UserId, request.Id, request.skip, request.take);

                if (result is null || !result.Any()) return Result< ScrollPage<UserResponseModel>>.Failuer(new Error("Not found", "not found"));

                return Result< ScrollPage<UserResponseModel>>.Success(new ScrollPage<UserResponseModel>
                {
                    Data = result,
                    Index = request.skip,
                    Limit = request.take,
                    
                });
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
