using Domain.Entities;
using Domain.Errors;
using Domain.ResponeModel;
using Domain.Ultils;
using Infrastructure.Unit0fWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Message
{
    public class GetMessageConversationCommand:IRequest<Result<PagingRespone<List<ClientMessageReceiver>>>>
    {
        public string? Id {  get; set; } 
        public int Skip {  get; set; }
        public int Limit {  get; set; }
    }
    public class HandGetMessageConversationCommand : IRequestHandler<GetMessageConversationCommand, Result<PagingRespone<List<ClientMessageReceiver>>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public HandGetMessageConversationCommand(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PagingRespone<List<ClientMessageReceiver>>>> Handle(GetMessageConversationCommand request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.messageRepository.GetMessagesAsync(request.Id!, request.Skip, request.Limit);

            if (!result.Any()) return Result<PagingRespone<List<ClientMessageReceiver>>>.Failuer(ConversationError.NotFound);

            return Result<PagingRespone<List<ClientMessageReceiver>>>.Success(new PagingRespone<List<ClientMessageReceiver>>
            {
                Index = request.Skip,
                Limit = request.Limit,
                Data = result
            });
        }
    }
}
