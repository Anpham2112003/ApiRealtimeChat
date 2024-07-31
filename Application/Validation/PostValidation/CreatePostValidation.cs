using Application.Features.Post;
using Domain.Ultils;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.PostValidation
{
    public class CreatePostValidation : AbstractValidator<CreatePostCommand>
    {
        public CreatePostValidation()
        {
            RuleFor(x => x.Content).Length(1, 255);

            RuleForEach(x => x.Files).ValidationFile(new[] { ".jpg", ".png" }, 10000);
        }
    }
}
