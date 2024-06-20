using Application.Features.Notification;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiRealtimeChat.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize]
    public class NotificationController:ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("notifications")]

        public async Task<IActionResult> GetNotification([FromQuery]int skip, int take)
        {
            var result = await _mediator.Send(new GetNotificationCommand(skip, take));

            return result.IsSuccess ? Ok(result.Data) : NotFound();
        }

        [HttpDelete("notification/delete/{id}")]
        public async Task<IActionResult> RemoveNotification(string id)
        {
            var result = await _mediator.Send(new RemoveNotificationCommand(id));

            return result.IsSuccess?Ok(result.Data) : BadRequest();
        }
    }
}
