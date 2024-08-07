﻿using Domain.Entities;
using Domain.Ultils;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Group
{
    public class CreateGroupCommand:IRequest<Result<ConversationCollection>>
    {
        public string? Name { get; set; }
       
    }

    public class HandCreateGroupCommand : IRequestHandler<CreateGroupCommand, Result<ConversationCollection>>
    {
        private readonly IHttpContextAccessor _context;
        private readonly IUnitOfWork _unitOfWork;
        

        public HandCreateGroupCommand(IHttpContextAccessor context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ConversationCollection>> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var UserId = _context.HttpContext!.User.FindFirstValue(ClaimTypes.PrimarySid);

                var group = new ConversationCollection
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Owners = new List<ObjectId> { ObjectId.Parse(UserId) },
                    IsGroup = true,
                    Messages = new List<Domain.Entities.Message>(),
                    Pinds = new List<Domain.Entities.Message>(),
                    Seen = DateTime.UtcNow,
                    CreatedAt=DateTime.UtcNow,
                    Group = new Domain.Entities.Group
                    {
                        Name = request.Name,
                        TotalMember = 1,
                        Avatar = "",
                        Members = new List<Member>
                        {
                            new Member(UserId,Domain.Enums.GroupRoles.Created)
                        },
                        UpdatedAt=DateTime.UtcNow,
                    }
                };

               

                await _unitOfWork.groupRepository.CreateGroupAsync(group);

                return Result<ConversationCollection>.Success(group);
            }
            catch (Exception)
            {

                throw;
            }
            
        }
    }
}
