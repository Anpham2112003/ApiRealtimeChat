using Application.Features.MessageGroup;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiRealtimeChat.Controllers
{
    [ApiController]
    [Route("api")]
    public class MessageGroupController:ControllerBase
    {
        private readonly IMediator _mediator;

        public MessageGroupController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("group/messages/{id}")]
        public async Task<IActionResult> GetMesssage(string id,[FromQuery]int page,int skip, int limit)
        {
            var result = await _mediator.Send(new GetMessageGroupCommand
            {
                GroupId = id,
                Limit = limit,
                Page = page,
                Skip = skip,
            });

            return result.IsSuccess ? Ok(result.Data) : NotFound(result.Error);
        }

        [Authorize]
        [HttpPost("group/message/insert")]
        public async Task< IActionResult> AddMessage(InsertMessageGroupCommand command)
        {
            var result  =await _mediator.Send(command);
            return result.IsSuccess?Ok(result.Data): BadRequest(result.Error);
        }

        [Authorize] 
        [HttpDelete("group/message/delete")]
        public async Task<IActionResult> RemoveMessage(RemoveMessageGroupCommand command)
        {
            await _mediator.Send(command);

            return Ok();
        }



    }
}
