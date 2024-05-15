using Application.Ultils;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.FriendMessage
{
    public class AddMessageToFriendCommand:IRequest<Result<string>>
    {
        public string? Id { get; set; }
        public MessageType Type { get; set; }
        public string? Content {  get; set; }


    }

    public class HandAddMessageToFriendCommand : IRequestHandler<AddMessageToFriendCommand, Result<string>>
    {
        public Task<Result<string>> Handle(AddMessageToFriendCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
