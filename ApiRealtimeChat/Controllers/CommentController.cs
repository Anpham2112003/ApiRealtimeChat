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


        [HttpGet("comments")]
        public async Task<IActionResult> GetComment([FromQuery]string AccountId , string PostId , int Skip,int Limit)
        {
            var result = await _mediator.Send(new GetCommentPost(AccountId,PostId,Skip,Limit));

            return result.IsSuccess ? Ok(result.Data) : NotFound();
        }

        [HttpGet("comment/reps")]
        public async Task<IActionResult> GetRepComment([FromQuery] string AccountId, string PostId, string ParentId, int Skip, int Limit)
        {
            var result = await _mediator.Send(new GetRepComment(AccountId, PostId, ParentId,Skip,Limit));

            return result.IsSuccess ? Ok(result.Data) : NotFound();
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

        [HttpDelete("comment/delete")]
        public async Task<IActionResult> UnHiddenComment(DeleteCommentCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok(result.Data) : BadRequest();
        }
    }
}
