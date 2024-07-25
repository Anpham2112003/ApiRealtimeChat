using Application.Features.Coment;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ApiRealtimeChat.Controllers
{
    [ApiController]
    [Route("api")]
    public class CommentController:ControllerBase
    {
        private readonly IMediator _mediator;

        public CommentController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("comment/push")]
        public async Task<IActionResult> PushComment([FromForm]CommentPostCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
        }

        [HttpPost("comment/rep")]
        public async Task<IActionResult> RepComment([FromForm]RepCommentPostCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
        }

        [HttpPost("comment/block")]
        public async Task<IActionResult> BlockComment(BlockCommentCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok(result.Data) : BadRequest();
        }

        [HttpPost("comment/unblock")]
        public async Task<IActionResult> UnBlockComment(UnBlockCommentCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok(result.Data) : BadRequest();
        }

        [HttpPost("comment/hidden")]
        public async Task<IActionResult> HiddenComment(HiddenCommentCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess?Ok(result.Data) : BadRequest();
        }

        [HttpPost("comment/unhidden")]
        public async Task<IActionResult> UnHiddenComment(UnHiddenCommentCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok(result.Data) : BadRequest();
        }
    }
}
