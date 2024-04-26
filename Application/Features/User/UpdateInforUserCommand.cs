using Application.Errors;
using Application.Ultils;
using Domain.Entites;
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
    public class UpdateInforUserCommand:IRequest<Result<UpdateInforUserCommand>>
    {
        public string? Id {  get; set; }
        public string? FistName {  get; set; }
        public string? LastName {  get; set; }
        public string? FullName {  get; set; }
        public bool Gender { get; set; }

        public DateTime UpdatedAt = DateTime.UtcNow;
    }

    public class HandUpdateInforUserCommand : IRequestHandler<UpdateInforUserCommand, Result<UpdateInforUserCommand>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public HandUpdateInforUserCommand(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<UpdateInforUserCommand>> Handle(UpdateInforUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var filter = Builders<UserCollection>.Filter.Eq(x => x.AccountId, ObjectId.Parse(request.Id));

                var update = Builders<UserCollection>.Update

                    .Set(x => x.FistName, request.FistName)
                    .Set(x => x.LastName, request.LastName)
                    .Set(x => x.FullName, request.FullName)
                    .Set(x => x.Gender, request.Gender)
                    .Set(x => x.UpdatedAt, request.UpdatedAt)
                    .ToBsonDocument();

                var result = await _unitOfWork.userRepository.UpdateAsync(filter, update);

                if (result.ModifiedCount == 0) return Result<UpdateInforUserCommand>.Failuer(UserError.UserNotFound(request.Id!));

                return Result<UpdateInforUserCommand>.Success(request);
            }
            catch (Exception)
            {

                throw;
            }

            

        }
    }
}
