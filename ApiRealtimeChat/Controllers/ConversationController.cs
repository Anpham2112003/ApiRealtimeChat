using Application.Features.Conversation;
using Application.Features.Message;
using MediatR;
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

        [HttpGet("conversation/{id}")]
        public async Task<IActionResult>GetSingleConversation(string id)
        {
            var result = await _mediator.Send(new GetConversationCommand() { Id = id });

            return Ok(result.Data);
        }

        [HttpGet("conversations")]
        public async Task<IActionResult> GetAllConversation([FromQuery]int skip,int limit)
        {
            var result = await _mediator.Send(new GetAllConversationCommand { skip=skip,limit = limit });

            return result.IsSuccess?Ok(result.Data):NotFound(result.Error);
        }

        [HttpDelete("conversation/delete/{id}")]
        public async Task<IActionResult> RemoveConversation(string id)
        {
            var result = await _mediator.Send(new RemoveConversationCommand() { Id = id }); 

            return result.IsSuccess ? Ok(result.Data) :BadRequest(result.Error);
        }
    }
}
