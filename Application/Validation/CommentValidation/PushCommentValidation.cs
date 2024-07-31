using Application.Features.Coment;
using Domain.Ultils;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.CommentValidation
{
    public class PushCommentValidation : AbstractValidator<CommentPostCommand>
    {
        public PushCommentValidation()
        {
            RuleFor(x=>x.AccountId).NotNull().NotEmpty();

            RuleFor(x=>x.PostId).NotNull().NotEmpty();

            RuleFor(x => x.Content).Length(1, 255);

            RuleFor(x => x.File).ValidationFile(new[] { ".jpg", ".png" }, 10000);

        }
    }
}
