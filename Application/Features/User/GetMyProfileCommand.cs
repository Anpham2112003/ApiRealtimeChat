using Application.Errors;
using Domain.Entities;
using Domain.Ultils;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.User
{
    public class GetMyProfileCommand:IRequest<Result<UserCollection>>
    {
       

       
    }

    public class HandGetInforUser : IRequestHandler<GetMyProfileCommand, Result<UserCollection>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        public HandGetInforUser(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<UserCollection>> Handle(GetMyProfileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _contextAccessor.HttpContext!.User.GetIdFromClaim();

                var user = await _unitOfWork.userRepository.FindUserByAccountId(userId);

                if (user is null) return Result<UserCollection>.Failuer(UserError.UserNotFound(userId));

                return Result<UserCollection>.Success(user);
            }
            catch (Exception)
            {

                throw;
            }
          
        }
    }
}
