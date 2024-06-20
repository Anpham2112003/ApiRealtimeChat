using Application.Features.Message;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.MessageValidation
{
    public class PindMessageValidation : AbstractValidator<PindMessageCommand>
    {
        public PindMessageValidation()
        {
            RuleFor(x=>x.ConversationId).NotNull().NotEmpty();

            RuleFor(x=>x.MessageId).NotNull().NotEmpty();
        }
    }
}
