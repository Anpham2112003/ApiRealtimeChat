using Domain.Entities;
using Domain.ResponeModel;
using Domain.Ultils;
using Infrastructure.Unit0fWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.User
{
    public class SearchUserCommand:IRequest<Result<List<UserConvert>>>
    {
        public string? name {  get; set; }

        public SearchUserCommand(string? name)
        {
            this.name = name;
        }
    }
    public class HandSearchUserCommand : IRequestHandler<SearchUserCommand, Result<List<UserConvert>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public HandSearchUserCommand(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<UserConvert>>> Handle(SearchUserCommand request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.userRepository.SearchUser(request.name!);

            if (result.Any()) return Result<List<UserConvert>>.Success(result);

            return Result<List<UserConvert>>.Failuer(new Error("Not found", ""));
        }
    }
}
