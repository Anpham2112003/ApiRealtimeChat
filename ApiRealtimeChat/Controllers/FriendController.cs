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

        [Authorize]
        [HttpGet("friend/search")]
        public async Task<IActionResult> Search([FromQuery]string name)
        {
            var result = await _mediator.Send(new SearchFriendCommand { Name = name });

            return result.IsSuccess ? Ok(result.Data) : NotFound();
        }

        [Authorize]

        [HttpGet("friends/group/{id}")]
        public async Task<IActionResult> GetFriendNotInGroup(string id,[FromQuery] int skip,int limit)
        {
            var result = await _mediator.Send(new GetFriendNotInGroupCommand(id, skip, limit));

            return result.IsSuccess?Ok(result.Data) : NotFound();
        }

        [Authorize]
        [HttpPost("friend/accept")]
        public async Task<IActionResult> AddFriend(AcceptFriendCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok() : BadRequest(result.Error);
        }

        [Authorize]
        [HttpPost("friend/invite")]
        public async Task<IActionResult> AddToWaitList(SendInviteFriendCommand command)
        {
            var result =  await _mediator.Send(command);

            return result.IsSuccess?Ok(result.Data):BadRequest(result.Error);
        }

        [Authorize]
        [HttpGet("friend/invites")]
        public async Task<IActionResult> GetFromWaiList([FromQuery]GetWaitListCommand command)
        {
            var result = await _mediator.Send(command);

            return result.Data is null?NotFound():Ok(result.Data);
        }

        [Authorize]

        [HttpGet("friends")]
        public async Task<IActionResult> GetFriend([FromQuery] int skip, int limit)
        {
            var result = await _mediator.Send(new GetFriendCommand( skip, limit));

            return result.IsSuccess ? Ok(result.Data) : NotFound(result.Error);
        }

        [Authorize]

        [HttpDelete("friend/cancelrequest/{id}")]
        public async Task<IActionResult> CancelFriendRequset(string id)
        {
            var result = await _mediator.Send(new CancelFriendRequestCommand { Id = id });

            return result.IsSuccess?Ok(result?.Data) : NotFound(result.Error);
        }

        [Authorize]

        [HttpDelete("friend/delete/{id}")]
        public async Task<IActionResult> RemoveFriend(string id)
        {
            var result = await _mediator.Send( new RemoveFriendCommand(id));

            return result.IsSuccess?Ok(result.Data) : BadRequest(result.Error);
        }

        [Authorize]
        [HttpDelete("friend/invite/delete/{id}")]
        public async Task<IActionResult> RefuseFriendRequest(string id)
        {
            var result = await _mediator.Send(new RefuseFriendRequestsCommand(id));

            return result.IsSuccess?Ok(result.Data): BadRequest(result.Error);  
        }
       
    }
}
