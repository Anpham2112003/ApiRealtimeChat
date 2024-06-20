using Application.Features.User;
using Infrastructure.Unit0fWork;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace ApiRealtimeChat.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize]
    public class UserController:ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("user/search")]
        public async Task<IActionResult> SearchUser([FromQuery]string name)
        {
            var result = await _mediator.Send(new SearchUserCommand(name));

            return result.IsSuccess ? Ok(result.Data) : NotFound();
        }

        [HttpGet("user/profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            var result  = await _mediator.Send(new GetMyProfileCommand());

            return result.IsFailuer ? NotFound(result.Error) : Ok(result.Data);
        }

        [HttpGet("user/profile/{id}")]
        public async Task<IActionResult> ViewProfile(string id)
        {
            var result = await _mediator.Send(new ViewProfileUserCommand(id));

            return result.IsSuccess?Ok(result.Data): NotFound();
        }

        [HttpPut("user/profile/edit")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileUserCommand command)
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

        [HttpDelete("user/avatar/delete")]
        public async Task<IActionResult> RemoveAvatar()
        {
            var result = await _mediator.Send(new RemoveAvatarUserCommand());

            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
        }
    }
}
