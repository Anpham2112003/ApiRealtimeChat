using Application.Features.Group;
using Domain.Ultils;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.GroupValidation
{
    public class UpdateAvatarGroupValidation : AbstractValidator<UpdateAvatarGroupCommand>
    {
        public UpdateAvatarGroupValidation()
        {
            RuleFor(x=>x.Id).NotEmpty().NotNull();
            RuleFor(X => X.File).ValidationFile(new[] {".jpg",".png"},10000);
        }
    }
}
