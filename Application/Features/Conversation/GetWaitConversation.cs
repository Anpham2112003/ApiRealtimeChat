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

namespace Application.Features.Conversation
{
    public class GetWaitConversation:IRequest<Result<ScrollPage<ConversationResponseModel>>>
    {
        public int skip {  get; set; }
        public int limit { get; set; }
    }
    public class HandGetWaitConversation : IRequestHandler<GetWaitConversation, Result<ScrollPage<ConversationResponseModel>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;

        public HandGetWaitConversation(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<ScrollPage<ConversationResponseModel>>> Handle(GetWaitConversation request, CancellationToken cancellationToken)
        {
            try
            {
                var AccountId = _contextAccessor.HttpContext!.User.GetIdFromClaim();

                var result = await _unitOfWork.conversationRepository.GetWaitConversationAsync(AccountId, request.skip, request.limit);

                if (result.Any()) return Result<ScrollPage<ConversationResponseModel>>.Success(new ScrollPage<ConversationResponseModel>
                {
                    Data = result,
                    Index = request.skip,
                    Limit = request.limit
                });

                return Result<ScrollPage<ConversationResponseModel>>.Failuer(new Error("NotFound", "Not found"));
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
