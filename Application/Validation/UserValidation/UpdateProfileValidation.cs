using Application.Features.User;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.UserValidation
{
    public class UpdateProfileValidation : AbstractValidator<UpdateProfileUserCommand>
    {
        public UpdateProfileValidation()
        {
            RuleFor(x => x.FistName).Length(1, 255);

            RuleFor(x=>x.LastName).Length(1, 255);

            RuleFor(x=>x.FullName).Length(1, 255);

        }
    }
}
