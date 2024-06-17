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
    public class GetNotificationCommand:IRequest<Result<PagingRespone<List<Domain.Entities.Notification>>>>
    {
        public int skip {  get; set; }
        public int take { get; set; }

        public GetNotificationCommand(int skip, int take)
        {
            this.skip = skip;
            this.take = take;
        }
    }

    public class HandGetNotificationCommand : IRequestHandler<GetNotificationCommand, Result<PagingRespone<List<Domain.Entities.Notification>>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;

        public HandGetNotificationCommand(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<PagingRespone<List<Domain.Entities.Notification>>>> Handle(GetNotificationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var UserId = _contextAccessor.HttpContext!.User.GetIdFromClaim();

                var result = await _unitOfWork.notificationRepository.GetNotification(UserId, request.skip, request.take);

                if (result is null) return Result<PagingRespone<List<Domain.Entities.Notification>>>.Failuer(new Error("NotFound", "Collection Not Found!"));

                return Result<PagingRespone<List<Domain.Entities.Notification>>>.Success(new PagingRespone<List<Domain.Entities.Notification>>
                {
                    Index = request.skip,
                    Limit = request.take,
                    Data = result.Notifications
                });
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
