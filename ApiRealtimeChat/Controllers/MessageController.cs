using Application.Features.Message;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ApiRealtimeChat.Controllers
{
    [ApiController]
    [Route("api")]
    public class MessageController:ControllerBase
    {
        private readonly IMediator _mediator;

        public MessageController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("message/send")]
        public async Task<IActionResult> GetConversationSingleChat([FromForm]SendMessageCommand command)
        {
            var result =await _mediator.Send(command);

            return result.IsSuccess ? Ok(result.Data) : BadRequest(result);
        }

     
    }
}
