using Application.Features.Group;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.GroupValidation
{
    public class LeaveGroupValidation : AbstractValidator<LeaveGroupCommand>
    {
        public LeaveGroupValidation()
        {
            RuleFor(x=>x.Id).NotNull().NotEmpty();
            
        }
    }
}
