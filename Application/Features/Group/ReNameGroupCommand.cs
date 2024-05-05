using Application.Ultils;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Group
{
    public class ReNameGroupCommand:IRequest<Result<string>>
    {
        public string? Name { get; set; }
    }

    
}
