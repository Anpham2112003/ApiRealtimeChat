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

        [HttpGet("user/profile/{id}")]
        public async Task<IActionResult> GetInforUser(string id)
        {
            var result  = await _mediator.Send(new GetInforUserCommand(id));

            return result.IsFailuer ? NotFound(result.Error) : Ok(result.Data);
        }

        [HttpPut("user/profile/edit")]
        public async Task<IActionResult> UpdateInforUser(UpdateInforUserCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsFailuer?BadRequest(result.Error):Ok(result.Data);

        }

        [HttpPatch("user/avatar/edit")]
        public async Task<IActionResult> UpdateAvatar([FromForm]UpdateAvatarUserCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsFailuer ? BadRequest(result.Error) : Ok(result.Data);
        }

        [HttpDelete("user/avatar/delete/{id}")]
        public async Task<IActionResult> RemoveAvatar(string id)
        {
            var result = await _mediator.Send(new RemoveAvatarUserCommand(id));

            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
        }
    }
}
