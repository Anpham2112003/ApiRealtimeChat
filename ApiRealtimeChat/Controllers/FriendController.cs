using Application.Features.Friend;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace ApiRealtimeChat.Controllers
{
    [ApiController]
    [Route("api/friend")]
    public class FriendController:ControllerBase
    {
        private readonly IMediator _mediator;

        public FriendController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddFriend(AddFriendCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok() : BadRequest(result.Error);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> RemoveFriend( RemoveFriendCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess?Ok(result.Data) : BadRequest(result.Error);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFriend(string id, [FromQuery]int skip ,int limit)
        {
            var result = await _mediator.Send(new GetFriendCommand(id,skip,limit));

             return result.IsSuccess?Ok(result.Data):NotFound(result.Error);    
        }
    }
}
