using Application.Errors;
using Application.Ultils;
using Domain.Entites;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.MessageGroup
{
    public class InsertMessageGroupCommand:IRequest<Result<string>>
    {
        public string? GroupId {  get; set; }
        public string? Content { get; set; }
    }

    public class HandInsertMessageCommand : IRequestHandler<InsertMessageGroupCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;

        public HandInsertMessageCommand(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<string>> Handle(InsertMessageGroupCommand request, CancellationToken cancellationToken)
        {
            var messsageCollection = await _unitOfWork.messageRepository.GetLastMessageCollection(ObjectId.Parse(request.GroupId));

            if(messsageCollection == null) return Result<string>.Failuer(MessageError.ColectionNotFound);

            var message = new Message
            {
                Id = ObjectId.GenerateNewId(),
                Content = request.Content,
                IsDelete = false,
                CreatedAt = DateTime.UtcNow,
                MessageType = Domain.Enums.MessageType.Message,
                UserId = ObjectId.Parse(_contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.PrimarySid))
            };

            if (messsageCollection.Count.Equals(1000))
            {
                await _unitOfWork.messageRepository
                    .AddMessageCollection(ObjectId.Parse(request.GroupId),messsageCollection.Page+1, message);
            }

            await _unitOfWork.messageRepository
                .AddMessageToPage(ObjectId.Parse(request.GroupId), messsageCollection.Page, messsageCollection.Count + 1, message);
           
            return Result<string>.Success("Success!");
        }
    }
}
