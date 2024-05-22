using Application.Errors;
using Domain.Entities;
using Domain.Ultils;
using Infrastructure.Unit0fWork;
using MediatR;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.User
{
    public class GetInforUserCommand:IRequest<Result<UserCollection>>
    {
        public string? Id { get; set; }

        public GetInforUserCommand(string id)
        {
            Id = id;
        }

       
    }

    public class HandGetInforUser : IRequestHandler<GetInforUserCommand, Result<UserCollection>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public HandGetInforUser(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<UserCollection>> Handle(GetInforUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _unitOfWork.userRepository.FindUserByAccountId(request.Id!);

                if (user is null) return Result<UserCollection>.Failuer(UserError.UserNotFound(request.Id!));

                return Result<UserCollection>.Success(user);
            }
            catch (Exception)
            {

                throw;
            }
          
        }
    }
}
