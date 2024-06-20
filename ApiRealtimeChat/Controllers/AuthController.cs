using Application.Features.Account;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace ApiRealtimeChat.Controllers
{
    [ApiController]
    [Route("api")]
    public class AuthController:ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]

        [HttpPost("auth/signup")]
        public async Task<IActionResult> CreateAccount(CreateAccountCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                if(result.Data is not  null)  return Ok(result.Data);

                return Ok("Success");
            }

            return BadRequest(result.Error);
        }

        [HttpPost("auth/signin")]
        public async Task<IActionResult> LoginAccount(LoginAccountCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsSuccess)  return Ok(result.Data);

            return Unauthorized(result.Error);
        }

        [HttpPost("auth/signout")]
        public async Task<IActionResult> Logout(LogoutAccountCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok(result.Data) : Unauthorized();
        }

        [HttpPost("auth/refreshtoken")]
        public async Task<IActionResult> RefreshToken(RefreshTokenCommmand commmand)
        {
            var result = await _mediator.Send(commmand);

           return result.IsSuccess?Ok(result.Data): Unauthorized();
        }

        [HttpPost("auth/resetpassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok() : BadRequest(result.Error);
        }

        [HttpGet("signin/google")]
        public async Task GoogleLogin()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,new AuthenticationProperties() 
            { 
                RedirectUri=Url.Action("GoogleCallback")
            });;
          
        }

        [HttpGet("google/callback")]
        public async Task<IActionResult> GoogleCallback()
        {
            var authenResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            if (!authenResult.Succeeded) return Unauthorized(authenResult.Failure?.Message);

            var fullname = authenResult.Principal.FindFirstValue(ClaimTypes.Name).Split(' ');

            var email = authenResult.Principal.FindFirstValue(ClaimTypes.Email);

            var result = await _mediator.Send(new GoogleLoginCommand()
            {
                FistName = fullname.First(),
                LastName = fullname.Last(),
                Email = email,
            });

            return Ok(result.Data);
        }

    

        [HttpGet("auth/resetpassword/callback")]
        public async Task<IActionResult> ResetPasswordCallback([FromQuery]ResetPasswordCallbackCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess?Ok(result.Data) : BadRequest(result.Error); 
        }

        [HttpPut("auth/changepassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess?Ok():BadRequest(result.Error);
        }

        [HttpDelete("auth/delete/account")]
        public async Task<IActionResult> RemoveAccount(RemoveAccountCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.IsSuccess) return BadRequest(result.Error);

            return Ok(result.Data);

        }
    }
}
