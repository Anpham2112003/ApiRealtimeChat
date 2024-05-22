using Application.Errors;
using Domain.Entities;
using Domain.Ultils;
using Infrastructure.Unit0fWork;
using MediatR;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Account
{
    public class ChangePasswordCommand:IRequest<Result<string>>
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? NewPassword { get; set; }

    }

    public class HandChangePasswordCommand : IRequestHandler<ChangePasswordCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public HandChangePasswordCommand(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var account = await _unitOfWork.accountRepository.FindAccountByEmail(request.Email!);

                if (account == null || HashLibrary.VerifyHash(account.Password!, request.Password!) is false)

                    return Result<string>.Failuer(AccountError.AccountIncorrect());


                var hashPassword = HashLibrary.GenerateHash(request.NewPassword!); 
                var filter = Builders<AccountCollection>.Filter.Eq(x => x.Email, request.Email);
                var update = Builders<AccountCollection>.Update.Set(x => x.Password, hashPassword);

                await _unitOfWork.accountRepository.UpdateAsync(filter, update);

                return Result<string>.Success();
            }
            catch (Exception)
            {

                throw;
            }
           

        }
    }
}
