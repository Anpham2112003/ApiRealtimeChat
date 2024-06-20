using Application.Features.User;
using Domain.Ultils;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.UserValidation
{
    public class UpdateAvatarValidation:AbstractValidator<UpdateAvatarUserCommand>
    {
        public UpdateAvatarValidation()
        {


            RuleFor(x => x.Image)
                .ValidationFile(new [] { ".png",".jpg"}, 10000);
        }
    }
}
