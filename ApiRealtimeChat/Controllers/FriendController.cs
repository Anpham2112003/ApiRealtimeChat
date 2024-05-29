using Application.Features.Friend;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace ApiRealtimeChat.Controllers
{
    [ApiController]
    [Route("api")]
    public class FriendController:ControllerBase
    {
        private readonly IMediator _mediator;

        public FriendController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("friend/accept")]
        public async Task<IActionResult> AddFriend(AddFriendCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok() : BadRequest(result.Error);
        }

        [Authorize]
        [HttpPost("friend/waitlist/add")]
        public async Task<IActionResult> AddToWaitList(AddToWaitListCommand command)
        {
            var result =  await _mediator.Send(command);

            return result.IsSuccess?Ok(result.Data):BadRequest(result.Error);
        }

        [Authorize]
        [HttpGet("friend/waitlist")]
        public async Task<IActionResult> GetFromWaiList([FromQuery]GetInforFriendFromWaitListCommand command)
        {
            var result = await _mediator.Send(command);

            return result.Data is null?NotFound():Ok(result.Data);
        }
        

        [HttpGet("friends/{id}")]
        public async Task<IActionResult> GetFriend(string id, [FromQuery] int skip, int limit)
        {
            var result = await _mediator.Send(new GetFriendCommand(id, skip, limit));

            return result.IsSuccess ? Ok(result.Data) : NotFound(result.Error);
        }

        [HttpDelete("friend/cancel/{id}")]
        public async Task<IActionResult> CancelFriendRequset(string id)
        {
            var result = await _mediator.Send(new CancelFriendRequestCommand { Id = id });

            return result.IsSuccess?Ok(result?.Data) : NotFound(result.Error);
        }

        [HttpDelete("friend/delete/{id}")]
        public async Task<IActionResult> RemoveFriend(string id)
        {
            var result = await _mediator.Send( new RemoveFriendCommand(id));

            return result.IsSuccess?Ok(result.Data) : BadRequest(result.Error);
        }
        [HttpDelete("friend/waitlist/delete/{id}")]
        public async Task<IActionResult> RejectFriend(string id)
        {
            var result = await _mediator.Send(new RejectFriendWaitListCommand(id));

            return result.IsSuccess?Ok(result.Data): BadRequest(result.Error);  
        }
       
    }
}
