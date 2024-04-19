using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entites;
using MongoDB.Bson;
using Infrastructure.Unit0fWork;
using AutoMapper;
using Application.Ultils;
using Application.Errors;
using MongoDB.Driver;

namespace Application.Features.Account
{
    public class CreateAccountCommand:IRequest<Result<CreateAccountCommand>>
    {
        public ObjectId Id = ObjectId.GenerateNewId();
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
        private readonly IMapper _mapper;

        public HandCreateAccountCommand(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<CreateAccountCommand>> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            
            try
            {

                var checkExist = await _unitOfWork.accountRepository.FindAccountByEmail(request.Email!);

                if (checkExist is not null) return Result<CreateAccountCommand>.Failuer(AccountError.AccountAllready(request.Email!));

                var account = _mapper.Map<AccountCollection>(request);

                var user = new UserCollection()
                {
                    Id = ObjectId.GenerateNewId(),
                    AccountId = account.Id,
                    Avatar = null,
                    FistName = request.FistName,
                    LastName = request.LastName,
                    FullName = request.FistName + request.LastName,
                    
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
