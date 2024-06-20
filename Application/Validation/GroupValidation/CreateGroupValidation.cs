using Application.Features.Group;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.GroupValidation
{
    public class CreateGroupValidation : AbstractValidator<CreateGroupCommand>
    {
        public CreateGroupValidation()
        {
            RuleFor(x => x.Name).Length(1, 255);
        }
    }
}
