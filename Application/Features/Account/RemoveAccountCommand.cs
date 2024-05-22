using Application.Errors;
using Domain.Ultils;
using Infrastructure.Unit0fWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Account
{
    public class RemoveAccountCommand:IRequest<Result<RemoveAccountCommand>>
    {
        public string? Email {  get; set; }
        public string? Password { get; set; }

        public RemoveAccountCommand(string? email, string? password)
        {
            Email = email;
            Password = password;
        }
    }

    public class HandRemoveAccountCommand : IRequestHandler<RemoveAccountCommand, Result<RemoveAccountCommand>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public HandRemoveAccountCommand(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<RemoveAccountCommand>> Handle(RemoveAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var account = await _unitOfWork.accountRepository.FindAccountByEmail(request.Email!);

                if (account is null) return Result<RemoveAccountCommand>.Failuer(AccountError.AccountIncorrect());

                var compare = HashLibrary.VerifyHash(account.Password!, request.Password!);

                if (!compare) return Result<RemoveAccountCommand>.Failuer(AccountError.AccountIncorrect());

                await _unitOfWork.accountRepository.SoftDeleteAccount(account);

                return Result<RemoveAccountCommand>.Success(request);
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
