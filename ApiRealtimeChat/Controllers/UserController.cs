using Application.Features.User;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace ApiRealtimeChat.Controllers
{
    [ApiController]
    [Route("api")]
    public class UserController:ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("user/info/{id}")]
        public async Task<IActionResult> GetInforUser(string id)
        {
            var result  = await _mediator.Send(new GetInforUserCommand(id));

            if (result.IsFailuer) return NotFound(result.Error);

            return Ok(result.Data);
        }
    }
}
