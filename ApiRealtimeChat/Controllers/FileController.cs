
using Domain.Ultils;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ApiRealtimeChat.Controllers
{
    [ApiController]
    [Route("api")]
    public class FileController:ControllerBase
    {
        private readonly IMediator _mediator;

        public FileController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("file")]
        public IActionResult GetFile([FromQuery] string path)
        {

            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), path);

            Response.Headers.ContentDisposition = $"attachment; filename=\"{Path.GetFileName(path)}\"";



           return File(new FileStream(fullPath,FileMode.Open), "application/octet-stream");
            
            
            

            
        }
    }
}
