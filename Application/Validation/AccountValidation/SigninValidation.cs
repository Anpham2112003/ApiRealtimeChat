using Application.Features.Account;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.AccountValidation
{
    public class SigninValidation : AbstractValidator<CreateAccountCommand>
    {
        public SigninValidation()
        {
            RuleFor(x => x.FistName)
                
                .Length(1, 255);

            RuleFor(x => x.LastName)
               
                .Length(1, 255);

            RuleFor(x => x.Email)
                
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty()
                .NotNull()
                .Length(8, 255);


        }
    }
}
