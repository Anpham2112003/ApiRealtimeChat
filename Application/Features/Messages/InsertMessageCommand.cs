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
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Messages
{
    public class InsertMessageCommand:IRequest<Result<string>>
    {
        public string? Id {  get; set; }
        public string? Content { get; set; }
    }

    public class HandInsertMessageCommand : IRequestHandler<InsertMessageCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;

        public HandInsertMessageCommand(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<string>> Handle(InsertMessageCommand request, CancellationToken cancellationToken)
        {
            var test = await _unitOfWork.messageRepository.GetLastMessageCollection(ObjectId.Parse(request.Id));

            var message = new Message
            {
                Id = ObjectId.GenerateNewId(),
                Content = "hahha",
                IsDelete = false,
                CreatedAt = DateTime.UtcNow,

            };

            await _unitOfWork.messageRepository.AddMessageToPage(ObjectId.Parse(request.Id),test.Page,message);
            return Result<string>.Success();
        }
    }
}
