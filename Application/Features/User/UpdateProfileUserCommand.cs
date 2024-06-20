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
    public class UpdateProfileUserCommand:IRequest<Result<UpdateProfileUserCommand>>
    {
       
        public string? FistName {  get; set; }
        public string? LastName {  get; set; }
        public string? FullName {  get; set; }
        public bool Gender { get; set; }

        public DateTime UpdatedAt = DateTime.UtcNow;
    }

    public class HandUpdateInforUserCommand : IRequestHandler<UpdateProfileUserCommand, Result<UpdateProfileUserCommand>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        public HandUpdateInforUserCommand(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<UpdateProfileUserCommand>> Handle(UpdateProfileUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _contextAccessor.HttpContext!.User.GetIdFromClaim();

                await _unitOfWork.userRepository.UpdateProfileUser(userId,request.ToBsonDocument());
               
                return Result<UpdateProfileUserCommand>.Success(request);
            }
            catch (Exception)
            {

                throw;
            }

            

        }
    }
}
