using Application.Features.Friend;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.FriendValidation
{
    public class RemoveFriendValidation : AbstractValidator<RemoveFriendCommand>
    {
        public RemoveFriendValidation()
        {
            RuleFor(x=>x.FriendId).NotNull().NotEmpty();
        }
    }
}
