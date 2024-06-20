using Application.Features.Account;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.AccountValidation
{
    public class LoginValidation : AbstractValidator<LoginAccountCommand>
    {
        public LoginValidation()
        {
            RuleFor(x => x.Email)
                .NotNull()
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotNull()
                .NotEmpty()
                .Length(8, 255);
        }
    }
}
