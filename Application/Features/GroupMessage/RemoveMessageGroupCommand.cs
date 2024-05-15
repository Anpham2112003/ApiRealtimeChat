using Application.Ultils;
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
    public class RemoveMessageGroupCommand:IRequest<Result<string>>
    {
        public string? Id {  get; set; }
        public int Page {  get; set; }
        public string? MessageId { get; set; }
    }

    public class HandRemoveMessageComamand : IRequestHandler<RemoveMessageGroupCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public HandRemoveMessageComamand(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(RemoveMessageGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.messageRepository.RemoveMessageAsync(ObjectId.Parse(request.Id), request.Page, ObjectId.Parse(request.MessageId));

                return Result<string>.Success();
            }
            catch (Exception)
            {

                throw;
            }
            

            
        }
    }
}
