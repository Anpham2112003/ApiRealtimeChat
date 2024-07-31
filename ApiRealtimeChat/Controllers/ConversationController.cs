using Application.Features.Conversation;
using Application.Features.Message;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiRealtimeChat.Controllers
{
    [ApiController]
    [Route("api")]
    public class ConversationController:ControllerBase
    {
        private readonly IMediator _mediator;

        public ConversationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpGet("conversation/friend/{id}")]
        public async Task<IActionResult>GetSingleConversation(string id)
        {
            var result = await _mediator.Send(new GetConversationCommand() { Id = id });

            return result.IsSuccess? Ok(result.Data) :BadRequest(result.Error);
        }

        [Authorize]
        [HttpGet("conversation/{id}")]
        public async Task<IActionResult> GetConversationById(string id)
        {
            var result = await _mediator.Send(new GetConversationByIdCommand() { Id = id });

            return result.IsSuccess ? Ok(result.Data) : NotFound();
        }

        [Authorize]
        [HttpGet("conversations")]
        public async Task<IActionResult> GetAllConversation([FromQuery]int skip,int limit)
        {
            var result = await _mediator.Send(new GetAllConversationCommand { skip=skip,limit = limit });

            return result.IsSuccess?Ok(result.Data):NotFound(result.Error);
        }

        [Authorize]
        [HttpGet("conversations/wait")]
        public async Task<IActionResult> GetWaitConversation([FromQuery] int skip, int limit)
        {
            var result = await _mediator.Send(new GetWaitConversation { skip = skip, limit = limit });

            return result.IsSuccess ? Ok(result.Data) : NotFound(result.Error);
        }

        [Authorize]
        [HttpPut("conversation/allow")]
        public async Task<IActionResult> GetWaitConversation(AllowConversationCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
        }

        [Authorize]
        [HttpDelete("conversation/delete/{id}")]
        public async Task<IActionResult> RemoveConversation(string id)
        {
            var result = await _mediator.Send(new RemoveConversationCommand() { Id = id }); 

            return result.IsSuccess ? Ok(result.Data) :BadRequest(result.Error);
        }
    }
}
