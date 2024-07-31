using Application.Features.Conversation;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.ConversationValidation
{
    public class AllowConversationValidation : AbstractValidator<AllowConversationCommand>
    {
        public AllowConversationValidation()
        {
            RuleFor(x=>x.ConversationId).NotNull().NotEmpty();
        }
    }
}
