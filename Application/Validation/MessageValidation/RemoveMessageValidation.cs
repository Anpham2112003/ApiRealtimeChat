using Application.Features.Message;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.MessageValidation
{
    public class RemoveMessageValidation : AbstractValidator<RemoveMessageCommand>
    {
        public RemoveMessageValidation()
        {
            RuleFor(x=>x.Id).NotNull().NotEmpty();
            RuleFor(x => x.MessageId).NotNull().NotEmpty();
        }
    }
}
