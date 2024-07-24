using Application.Features.Message;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.MessageValidation
{
    public class ValidationMessage : AbstractValidator<SendMessageCommand>
    {
        public ValidationMessage()
        {
            RuleFor(x=>x.Id)
                .NotNull()
                .NotEmpty();

            RuleFor(x=>x.Content)
                .NotEmpty()
                .NotNull();
        }
    }
}
