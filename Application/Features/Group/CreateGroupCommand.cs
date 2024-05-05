using Application.Ultils;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Group
{
    public class CreateGroupCommand:IRequest<Result<string>>
    {
        public string? GroupName { get; set;}
    }

    public class HandCreateGroupCommand : IRequestHandler<CreateGroupCommand, Result<string>>
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUnitOfWork _unitOfWork;

        public HandCreateGroupCommand(IHttpContextAccessor contextAccessor, IUnitOfWork unitOfWork)
        {
            _contextAccessor = contextAccessor;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var Id = _contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.PrimarySid);

                await _unitOfWork.groupRepository.CreateGroup(ObjectId.Parse(Id), request.GroupName!);

                return Result<string>.Success();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
