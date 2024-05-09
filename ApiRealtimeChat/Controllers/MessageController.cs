using Application.Features.Messages;
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

        [HttpPost("test")]
        public async Task< IActionResult> test(InsertMessageCommand command)
        {
            var result  =await _mediator.Send(command);
            return Ok(result);
        }
    }
}
