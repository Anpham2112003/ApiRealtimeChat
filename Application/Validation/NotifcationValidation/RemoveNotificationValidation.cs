using Application.Features.Notification;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.NotifcationValidation
{
    public class RemoveNotificationValidation : AbstractValidator<RemoveNotificationCommand>
    {
        public RemoveNotificationValidation()
        {
            RuleFor(x => x.Id).NotEmpty().NotNull();
        }
    }
}
