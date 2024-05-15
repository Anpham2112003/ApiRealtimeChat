using Application.Errors;
using Application.Ultils;
using Domain.ResponeModel;
using Infrastructure.Unit0fWork;
using MediatR;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.MessageGroup
{
    public class GetMessageGroupCommand:IRequest<Result<MessageResponeModel>>
    {
        public string? GroupId {  get; set; }
        public int Page {  get; set; }
        public int Skip {  get; set; }
        public int Limit {  get; set; }

    }

    public class HandGetMessageCommand : IRequestHandler<GetMessageGroupCommand, Result<MessageResponeModel>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public HandGetMessageCommand(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<MessageResponeModel>> Handle(GetMessageGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                 var result=await  _unitOfWork.messageRepository.GetMessageAsync(ObjectId.Parse(request.GroupId),request.Page,request.Skip,request.Limit);

                if (result is null) return Result<MessageResponeModel>.Failuer(MessageError.NotFound);

                return Result<MessageResponeModel>
                    .Success(new MessageResponeModel { Data = result.Messages, Page = request.Page, Skip = request.Skip, Limit = request.Limit });
            }
            catch (Exception)
            {

                throw;
            }
          
        }
    }
}
