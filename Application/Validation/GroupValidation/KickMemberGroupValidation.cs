using Application.Features.Group;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.GroupValidation
{
    public class KickMemberGroupValidation : AbstractValidator<KickMemberInGroupCommand>
    {
        public KickMemberGroupValidation()
        {
            RuleFor(x => x.Id).NotNull().NotEmpty();
            RuleFor(x=>x.MemberId).NotEmpty().NotNull();
        }
    }
}
