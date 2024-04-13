
using Application.Features.Account;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ApiRealtimeChat.Controllers
{
    [ApiController]
    public class TestController:ControllerBase
    {
        private readonly IMediator _mediator;

        public TestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("test")]
        public async Task<IActionResult> Test(CreateAccountCommand command)
        {
            var result  = await _mediator.Send(command);

            return Ok(result);
        }
    }
}
