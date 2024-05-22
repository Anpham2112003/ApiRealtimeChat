using Application.Features.Group;
using Domain.Ultils;
using MediatR;
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

        [HttpPost("create")]
        public async Task<IActionResult> CreateGroup(CreateGroupCommand command)
        {
            var result = await _mediator.Send(command);

            return Ok(result.Data);
        }

        [HttpPost("member/add")]
        public async Task<IActionResult> AddManyMemberToGroup(AddMemberToGroupComand comand)
        {
            var result = await _mediator.Send(comand);
            
            return result.IsSuccess?Ok(result.Data): BadRequest(result.Error);   
        }

        [HttpPut("avatar/change")]
        public async Task<IActionResult> UpdateAvartaGroup([FromForm] UpdateAvatarGroupCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> LeavGroup(string id)
        {
            var result = await _mediator.Send(new LeaveGroupCommand() { Id = id });

            return result.IsSuccess?Ok(result.Data):BadRequest(result.Error);
        }

        [HttpDelete("member/kick")]
        public async Task<IActionResult> KickMember(KickMemberInGroupCommand command)
        {
            var result =  await _mediator.Send(command);

            return result.IsSuccess?Ok(result.Data):BadRequest(result.Error);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteGroup(DeleteGroupCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess?Ok("Ok!"):BadRequest(result.Error);
        }

        

        
    }
}
