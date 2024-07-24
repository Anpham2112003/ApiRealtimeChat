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
    public class GetMessageConversationCommand:IRequest<Result<ScrollPage<ClientMessageResponseModel>>>
    {
        public string? Id {  get; set; } 
        public int Skip {  get; set; }
        public int Limit {  get; set; }
    }
    public class HandGetMessageConversationCommand : IRequestHandler<GetMessageConversationCommand, Result<ScrollPage<ClientMessageResponseModel>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public HandGetMessageConversationCommand(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ScrollPage<ClientMessageResponseModel>>> Handle(GetMessageConversationCommand request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.messageRepository.GetMessagesAsync(request.Id!, request.Skip, request.Limit);

            if (!result.Any()) return Result<ScrollPage<ClientMessageResponseModel>>.Failuer(ConversationError.NotFound);

            return Result<ScrollPage<ClientMessageResponseModel>>.Success(new ScrollPage<ClientMessageResponseModel>
            {
                Index = request.Skip,
                Limit = request.Limit,
                Data = result
            });
        }
    }
}
