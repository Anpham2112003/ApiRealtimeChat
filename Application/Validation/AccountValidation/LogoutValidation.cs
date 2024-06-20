using Application.Features.Account;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.AccountValidation
{
    public class LogoutValidation : AbstractValidator<LogoutAccountCommand>
    {
        public LogoutValidation()
        {
            RuleFor(x => x.token)
                .NotEmpty()
                .NotNull();
        }
    }
}
