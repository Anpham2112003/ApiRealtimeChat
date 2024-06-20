using Application.Features.Message;
using Domain.Enums;
using Domain.Ultils;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.MessageValidation
{
    public class ValidationMessageFile:AbstractValidator<SendFileCommand>
    {
        public ValidationMessageFile()
        {
            RuleFor(x=>x.Id).NotNull().NotEmpty();

            RuleFor(x => x.Type).IsInEnum();

            RuleFor(x => x.File).ValidationFile(new[] {".png",".jpg",".rar",".zip",".7z"},10240);
        }
    }
}
