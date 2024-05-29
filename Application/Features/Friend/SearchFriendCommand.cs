using Domain.Entities;
using Domain.ResponeModel;
using Domain.ResponeModel.BsonConvert;
using Domain.Ultils;
using Infrastructure.Unit0fWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Friend
{
    public class SearchFriendCommand:IRequest<Result<UserConvert>>
    {
        public string? Name { get; set; }
        
    }

    public class HandSearchFriendCommand : IRequestHandler<SearchFriendCommand, Result<UserConvert>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public HandSearchFriendCommand(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<Result<UserConvert>> Handle(SearchFriendCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
