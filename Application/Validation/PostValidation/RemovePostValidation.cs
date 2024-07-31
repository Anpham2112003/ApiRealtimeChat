using Application.Features.Post;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.PostValidation
{
    public class RemovePostValidation:AbstractValidator<RemovePostCommand>
    {
        public RemovePostValidation()
        {
            RuleFor(x=>x.PostId).NotNull().NotEmpty();
            
        }
    }
}
