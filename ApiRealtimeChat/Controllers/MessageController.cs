﻿using Application.Features.Message;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ApiRealtimeChat.Controllers
{
    [ApiController]
    [Route("api")]
    public class MessageController:ControllerBase
    {
        private readonly IMediator _mediator;

        public MessageController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("messages/{id}")]
        public async Task<IActionResult> GetMessage(string id,[FromQuery] int skip, int limit)
        {
            var result = await _mediator.Send(new GetMessageConversationCommand { Id = id, Skip = skip, Limit = limit });

            return result.IsSuccess ? Ok(result.Data) : NotFound(result.Error);
        }


        [HttpPost("message/send")]
        public async Task<IActionResult> GetConversationSingleChat([FromForm]SendMessageCommand command)
        {
            var result =await _mediator.Send(command);

            return result.IsSuccess ? Ok(result.Data) : BadRequest(result);
        }

        [HttpPut("message/pind")]
        public async Task<IActionResult> PindMessage(PindMessageCommand command)
        {
            var result = await _mediator.Send(command);
            
            return result.IsSuccess?Ok(result.Data) : BadRequest(result.Error);
        }

        [HttpDelete("message/unpind")]
        public async Task<IActionResult> UnPindMessage(UnPindMessageCommand command)
        {
            var result  =  await _mediator.Send(command);

            return result.IsSuccess?Ok(result.Data):BadRequest(result.Error);
        }

        [HttpPut("message/update")]
        public async Task<IActionResult> ChnageMessage(ChangeContentMessageCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok(result.Data) : BadRequest();
        }

        [HttpDelete("message/remove")]
        public async Task<IActionResult> RemoveMessage(RemoveMessageCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
        }

     
    }
}
