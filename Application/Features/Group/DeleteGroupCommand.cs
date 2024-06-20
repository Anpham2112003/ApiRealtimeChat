using Domain.Enums;
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
    public class DeleteGroupCommand:IRequest<Result<string>>
    {
        public string? Id {  get; set; }

        public DeleteGroupCommand(string? id)
        {
            Id = id;
        }
    }

    public class HandDeleteGroupCommand : IRequestHandler<DeleteGroupCommand, Result<string>>
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly IUnitOfWork _unitOfWork;

        public HandDeleteGroupCommand(IHttpContextAccessor httpContext, IUnitOfWork unitOfWork)
        {
            _httpContext = httpContext;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
        {
            var User = await _unitOfWork.groupRepository
                .GetMemberInGroup(request.Id!, _httpContext.HttpContext!.User.FindFirstValue(ClaimTypes.PrimarySid)); 

            if(User is null || !User.Role.Equals(GroupRoles.Created)) return Result<string>.Failuer(GroupError.NotPermission);

            var Ids = await _unitOfWork.groupRepository.DeleteGroupAsync(request.Id!);


            return Result<string>.Success();
        }
    }
}
