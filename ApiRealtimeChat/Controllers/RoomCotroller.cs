using Application.Features.GroupRoom;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiRealtimeChat.Controllers
{
    [ApiController]
    [Route("api")]
    public class RoomCotroller:ControllerBase
    {
        private readonly IMediator _mediator;

        public RoomCotroller(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpGet("room/group")]
        public async Task<IActionResult> GetGroup([FromQuery] int skip,int limit)
        {
            var result = await _mediator.Send(new GetGroupRoomCommand(skip, limit));

            return result.IsSuccess ? Ok(result.Data) : NotFound(result.Error);
        }
       
    }
}
