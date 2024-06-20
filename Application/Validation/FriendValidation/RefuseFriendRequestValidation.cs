﻿using Application.Features.Friend;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.FriendValidation
{
    public class RefuseFriendRequestValidation : AbstractValidator<RefuseFriendRequestsCommand>
    {
        public RefuseFriendRequestValidation()
        {
            RuleFor(x=>x.Id).NotEmpty().NotNull();

        }
    }
}
