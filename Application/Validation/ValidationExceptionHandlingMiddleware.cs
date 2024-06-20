using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation
{
    public class ValidationExceptionHandlingMiddleware : IMiddleware
    {
		

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
			try
			{
				await next(context);
			}
			catch (ValidationException exception)
			{

				context.Response.StatusCode = StatusCodes.Status400BadRequest;
				context.Response.Headers.ContentType = "application/json";
				await context.Response.WriteAsJsonAsync(new
				{
					exception.Errors
				});
			}
			catch (Exception )
			{
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.Headers.ContentType = "application/json";
                
            }
        }
    }
}
