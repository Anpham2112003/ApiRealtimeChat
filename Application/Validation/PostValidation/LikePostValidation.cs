using Application.Features.Post;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.PostValidation
{
    public class LikePostValidation : AbstractValidator<LikePostCommand>
    {
        public LikePostValidation()
        {
            RuleFor(x=>x.AccountId).NotNull().NotEmpty();
            RuleFor(x=>x.PostId).NotNull().NotEmpty();
        }
    }
}
