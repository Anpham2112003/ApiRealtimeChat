using Application.Features.Group;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiRealtimeChat.Controllers
{
    [ApiController]
    [Route("api")]
    public class GroupController:ControllerBase
    {
        private readonly IMediator _mediator;

        public GroupController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("group/member/{id}")]
        public async Task<IActionResult> GetMemberInGroup(string id, [FromQuery] int skip, int take)
        {
            var result = await _mediator.Send(new GetMemberInGroupCommand(id, skip, take));

            return result.IsSuccess ? Ok(result.Data) : NotFound(result.Error);
        }

        [Authorize]
        [HttpPost("group/create")]
        public async Task<IActionResult> CreateGroup(CreateGroupCommand command)
        {
            var result = await _mediator.Send(command);

            return Ok(result.Data);
        }

        [Authorize]
        [HttpPost("group/member/add")]
        public async Task<IActionResult> AddMemberToGroup(AddMemberToGroup command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
        }

        [Authorize]
        [HttpPut("group/rename")]
        public async Task<IActionResult> RenameGroup(ReNameGroupCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
        }

        [Authorize]
        [HttpPut("group/avatar")]
        public async Task<IActionResult> UpdateAvatarGroup(UpdateAvatarGroupCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
        }

        [Authorize]
        [HttpPut("group/member/role")]
        public async Task<IActionResult> UpdateRoleMember(ChangeRolesInGroupCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess?Ok(result.Data) : BadRequest(result?.Error);
        }

        [Authorize]
        [HttpDelete("group/avatar/delete")]
        public async Task<IActionResult> RemoveAvatarGroup(RemoveAvatarGroupCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess?Ok(result.Data) : BadRequest(result.Error);
        }

        [Authorize]
        [HttpDelete("group/member/kick")]
        public async Task<IActionResult> RemoveMemberInGroup(KickMemberInGroupCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess?Ok(result.Data):BadRequest(result?.Error);
        }

        [Authorize]
        [HttpDelete("group/member/leave")]
        public async Task<IActionResult> RemoveGroup(string id)
        {
            var result = await _mediator.Send(new LeaveGroupCommand(id));

            return result.IsSuccess?Ok(result.Data):BadRequest(result.Error);   
        }

        
    }
}
