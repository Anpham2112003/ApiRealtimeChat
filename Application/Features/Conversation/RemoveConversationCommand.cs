﻿using Domain.Enums;
using Domain.Errors;
using Domain.ResponeModel;
using Domain.Ultils;
using Infrastructure.Services.HubServices;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Conversation
{
    public class RemoveConversationCommand:IRequest<Result<string>>
    {
        public string? Id { get; set; }
    }

    public class HandRemoveConversation : IRequestHandler<RemoveConversationCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IHubContext<HubService,IHubServices> _hubContext;
        public HandRemoveConversation(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor, IHubContext<HubService, IHubServices> hubContext)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
            _hubContext = hubContext;
        }

        public async Task<Result<string>> Handle(RemoveConversationCommand request, CancellationToken cancellationToken)
        {
            try
            {
               var UserId = _contextAccessor.HttpContext!.User.GetIdFromClaim();

               var result = await _unitOfWork.conversationRepository.RemoveConversation(request.Id!,UserId);

                if (result.ModifiedCount == 0) return Result<string>.Failuer(new Error("", ""));
                
                var message = new Event
                {
                    EventType = EventType.DeleteConversation,

                    EventMessage = request.Id,
                };

                await _hubContext.Clients.User(UserId).Event(message);
                    
                
                return Result<string>.Success(request.Id);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
