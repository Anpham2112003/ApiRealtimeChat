using Application.Features.Friend;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.FriendValidation
{
    public class AccepectFriendValidation : AbstractValidator<AcceptFriendCommand>
    {
        public AccepectFriendValidation()
        {
            RuleFor(x=>x.Id).NotEmpty().NotNull();
           
        }
    }
}
