using Domain.Entities;
using Domain.ResponeModel;
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
    public class GetNotificationCommand:IRequest<Result<ScrollPage<NotificationResponseModel>>>
    {
        public int skip {  get; set; }
        public int take { get; set; }

        public GetNotificationCommand(int skip, int take)
        {
            this.skip = skip;
            this.take = take;
        }
    }

    public class HandGetNotificationCommand : IRequestHandler<GetNotificationCommand, Result<ScrollPage<NotificationResponseModel>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;

        public HandGetNotificationCommand(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<ScrollPage<NotificationResponseModel>>> Handle(GetNotificationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var UserId = _contextAccessor.HttpContext!.User.GetIdFromClaim();

                var result = await _unitOfWork.notificationRepository.GetNotification(UserId, request.skip, request.take);

                if (result is null || !result.Any()) return Result<ScrollPage<NotificationResponseModel>>.Failuer(new Error("NotFound", "Notification not found"));

                return Result<ScrollPage<NotificationResponseModel>>.Success(new ScrollPage<NotificationResponseModel>
                {
                    Index = request.skip,
                    Limit = request.take,
                    Data = result
                });
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
