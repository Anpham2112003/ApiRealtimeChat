using Application.Features.Post;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiRealtimeChat.Controllers
{
    [ApiController]
    [Route("api")]
    public class PostController:ControllerBase
    {
        private readonly IMediator _mediator;

        public PostController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("posts/latest")]
        public async Task<IActionResult> GetNewPost([FromQuery] int skip,int limit)
        {
            var result = await _mediator.Send(new GetNewPosts(skip,limit));

            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
        }
        [HttpGet("posts")]
        public async  Task<IActionResult>GetPostById ([FromQuery]string Id , int Skip,int Limit) 
        {
            var result = await _mediator.Send(new GetPostsById(Id, Skip,Limit));

            return result.IsSuccess?Ok(result.Data) : NotFound();
        }

        [HttpGet("post/likes")]
        public async Task<IActionResult> GetListLike([FromQuery]string AccountId, int PageId,string PostId,int Skip,int Limit)
        {
            var result = await _mediator.Send(new GetListUserLike
            {
                AccountId = AccountId,
                PostId = PostId,
                PageId = PageId,
                Skip = Skip,
                Limit = Limit
            });

            return result.IsSuccess?Ok(result.Data) : NotFound();
        }

        [Authorize]
        [HttpPost("post/create")]
        public async Task<IActionResult> CreatePost([FromForm]CreatePostCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok("success!") : BadRequest();
        }

        [Authorize]
        [HttpPost("post/like")]
        public async Task<IActionResult> LikePost(LikePostCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok("success") : BadRequest(result.Error);
        }

        [Authorize]
        [HttpPost("post/unlike")]
        public async Task<IActionResult> UnLikePost(UnLikePostCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok("success") : BadRequest(result.Error);
        }

        [HttpDelete("post/delete")]
        public async Task<IActionResult> RemovePost(RemovePostCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess?Ok(result.Data):BadRequest(result.Error);
        }
    }
}
