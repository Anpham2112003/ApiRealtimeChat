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

namespace Application.Features.Message
{
    public class GetMessagesPindCommand:IRequest<Result<PagingRespone<List<ClientMessageReceiver>>>>
    {
        public string? ConversationId { get; set; }
        public int Skip {  get; set; }
        public int  Limit { get; set; }


    }

    public class HandGetMessagesPindCommand : IRequestHandler<GetMessagesPindCommand, Result<PagingRespone<List<ClientMessageReceiver>>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;

        public HandGetMessagesPindCommand(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<PagingRespone<List<ClientMessageReceiver>>>> Handle(GetMessagesPindCommand request, CancellationToken cancellationToken)
        {
            var userId = _contextAccessor.HttpContext!.User.GetIdFromClaim();

            var result = await _unitOfWork.messageRepository.GetMessagesPind(request.ConversationId!,userId, request.Skip, request.Limit);

            if (result == null) return Result<PagingRespone<List<ClientMessageReceiver>>>.Failuer(new Error("Not found", "Not found"));

            return Result<PagingRespone<List<ClientMessageReceiver>>>.Success(new PagingRespone<List<ClientMessageReceiver>>
            {
                Data = result,
                Index = request.Skip,
                Limit = request.Limit
            });


        }
    }
}
