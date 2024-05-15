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
                var userId = ObjectId.Parse(_contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.PrimarySid));

                var GroupId =  await _unitOfWork.groupRepository.CreateGroup(userId, request.GroupName!);

                await _unitOfWork.groupRoomRepository.AddToGroupRoom(userId, GroupId);

                return Result<string>.Success();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
