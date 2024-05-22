using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using Infrastructure.Unit0fWork;
using AutoMapper;
using Application.Errors;
using MongoDB.Driver;
using Domain.Entities;
using Domain.Ultils;

namespace Application.Features.Account
{
    public class CreateAccountCommand:IRequest<Result<CreateAccountCommand>>
    {
        public string Id = ObjectId.GenerateNewId().ToString();
        public string? FistName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public DateTime Created => DateTime.UtcNow;
        public bool IsDelete => false;

      
    }

    public class HandCreateAccountCommand : IRequestHandler<CreateAccountCommand, Result<CreateAccountCommand>>
    {
        private readonly IUnitOfWork _unitOfWork;
       

        public HandCreateAccountCommand(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
          
        }

        public async Task<Result<CreateAccountCommand>> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            
            try
            {

                var checkExist = await _unitOfWork.accountRepository.FindAccountByEmail(request.Email!);

                if (checkExist is not null) return Result<CreateAccountCommand>.Failuer(AccountError.AccountAllready(request.Email!));

                var account = new AccountCollection
                {
                    Id = request.Id,
                    AccountType = Domain.Enums.AccountType.None,
                    CreatedAt = DateTime.UtcNow,
                    Email = request.Email,
                    Password = request.Password,
                };

                var user = new UserCollection()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    AccountId = account.Id.ToString(),
                    Avatar = null,
                    FistName = request.FistName,
                    LastName = request.LastName,
                    FullName = request.FistName + request.LastName,
                    State = Domain.Enums.UserState.Offline
                };
                     
                await _unitOfWork.accountRepository.InsertAsync(account);

                await _unitOfWork.userRepository.InsertAsync(user);

                return Result<CreateAccountCommand>.Success(request);
            }
            catch (Exception)
            {
               
                throw;
            }
           
              
        }
    
    }
}
