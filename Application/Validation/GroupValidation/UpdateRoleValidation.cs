using Application.Features.Group;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.GroupValidation
{
    public class UpdateRoleValidation : AbstractValidator<UpdateRoleGroupCommand>
    {
        public UpdateRoleValidation()
        {
            RuleFor(x=>x.GroupId).NotEmpty().NotNull();
            RuleFor(x=>x.MemberId).NotNull().NotEmpty();
            RuleFor(x => x.Role).IsInEnum();
        }
    }
}
