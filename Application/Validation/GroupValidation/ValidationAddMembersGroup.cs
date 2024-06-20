using Application.Features.Group;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.GroupValidation
{
    public class ValidationAddMembersGroup : AbstractValidator<AddMemberToGroupComand>
    {
        public ValidationAddMembersGroup()
        {
            RuleFor(x=>x.Id).NotNull().NotEmpty();
            
            RuleForEach(x=>x.MemberId).NotNull().NotEmpty();
        }
    }
}
