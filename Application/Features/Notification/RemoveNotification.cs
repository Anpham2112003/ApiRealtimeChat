using Domain.Ultils;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Notification
{
    public class RemoveNotificationCommand:IRequest<Result<string>>
    {
        public string? Id { get; set; }

        public RemoveNotificationCommand(string? id)
        {
            Id = id;
        }
    }
    public class HandRemoveNotificationCommand : IRequestHandler<RemoveNotificationCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;

        public HandRemoveNotificationCommand(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<string>> Handle(RemoveNotificationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var UserId = _contextAccessor.HttpContext!.User.GetIdFromClaim();

                var result = await _unitOfWork.notificationRepository.RemoveNotification(UserId, request.Id!);

                if (result.MatchedCount.Equals(0)) return Result<string>.Failuer(new Error("NotFound", "Not found!"));

                return Result<string>.Success("Ok!");
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
