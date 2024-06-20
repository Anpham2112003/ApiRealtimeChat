using Application.Features.Conversation;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.ConversationValidation
{
    public class DeleteConversationValidation : AbstractValidator<RemoveConversationCommand>
    {
        public DeleteConversationValidation()
        {
        }
    }
}
