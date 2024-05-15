using Application.Ultils;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.MessageGroup
{
    public class UpdateContentMessageGroupCommand:IRequest<Result<string>>
    {
        public string? Id {  get; set; }
        public int Page {  get; set; }
        public string? MessageId {  get; set; }
        public string? Content { get; set; }
        
    }

    public class HandUpdateContentMessageCommand : IRequestHandler<UpdateContentMessageGroupCommand, Result<string>>
    {
        public Task<Result<string>> Handle(UpdateContentMessageGroupCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
