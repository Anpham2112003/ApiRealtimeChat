using Application.Features.Account;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.AccountValidation
{
    public class ResetPasswordValidation : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordValidation()
        {
            RuleFor(x => x.Email)
                .NotNull()
                .NotEmpty()
                .EmailAddress();


        }
    }
}
