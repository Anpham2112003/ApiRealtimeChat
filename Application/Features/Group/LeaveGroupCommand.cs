using Domain.Errors;
using Domain.Ultils;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Group
{
    public class LeaveGroupCommand:IRequest<Result<string>>
    {
        public string? Id {  get; set; }
    }
    public class HandLeavGroupCommand : IRequestHandler<LeaveGroupCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _accessor;
        public HandLeavGroupCommand(IUnitOfWork unitOfWork, IHttpContextAccessor accessor)
        {
            _unitOfWork = unitOfWork;
            _accessor = accessor;
        }

        public async Task<Result<string>> Handle(LeaveGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var UserId = _accessor.HttpContext!.User.FindFirstValue(ClaimTypes.PrimarySid);

                var result = await _unitOfWork.groupRepository.LeaveGroup(request.Id!, UserId);

                if (result.MatchedCount == 0) return Result<string>.Failuer(GroupError.GroupNotFound);

                return Result<string>.Success(UserId);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
