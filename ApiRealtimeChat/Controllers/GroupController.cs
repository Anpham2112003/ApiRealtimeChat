using Application.Features.Group;
using Domain.Ultils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiRealtimeChat.Controllers
{
    [ApiController]
    [Route("api/group")]
    public class GroupController:ControllerBase
    {
        private readonly IMediator _mediator;

        public GroupController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpGet("{id}/members")]
        public async Task<IActionResult> GetMemberInGroup(string id,[FromQuery]int skip,int limit)
        {
            var result = await _mediator.Send(new GetMemberInGroupCommand
            {
                Id = id,
                Skip = skip,
                Limit = limit
            });

            return result.IsSuccess ? Ok(result) : NotFound();
        }


        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateGroup(CreateGroupCommand command)
        {
            var result = await _mediator.Send(command);

            return Ok(result.Data);
        }

        [Authorize]
        [HttpPost("member/insert")]
        public async Task<IActionResult> AddManyMemberToGroup(AddMemberToGroupComand comand)
        {
            var result = await _mediator.Send(comand);
            
            return result.IsSuccess?Ok(result.Data): BadRequest(result.Error);   
        }
        [Authorize]
        [HttpPost("member/role/change")]
        public async Task<IActionResult> UpdateRole(UpdateRoleGroupCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess?Ok(result.Data) : BadRequest(result.Error);
        }

        [Authorize] 
        [HttpPut("avatar/change")]
        public async Task<IActionResult> UpdateAvartaGroup([FromForm] UpdateAvatarGroupCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
        }

        [Authorize]
        [HttpDelete("leave/{id}")]
        public async Task<IActionResult> LeavGroup(string id)
        {
            var result = await _mediator.Send(new LeaveGroupCommand() { Id = id });

            return result.IsSuccess?Ok(result.Data):BadRequest(result.Error);
        }

        [Authorize]
        [HttpDelete("member/kick")]
        public async Task<IActionResult> KickMember(KickMemberInGroupCommand command)
        {
            var result =  await _mediator.Send(command);

            return result.IsSuccess?Ok(result.Data):BadRequest(result.Error);
        }

        [Authorize]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteGroup(DeleteGroupCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess?Ok("Ok!"):BadRequest(result.Error);
        }

        

        
    }
}
