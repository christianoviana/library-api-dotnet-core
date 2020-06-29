using Library.API.Domain.Results.Exceptions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Library.API.Middleware.Exceptions
{
    public class GlobalExceptionMiddleware
    {
        RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {                
                await _next(context);
            }
            catch (Exception ex)
            {
                await RespondeErrorAsync(context, ex);
            }
        }

        public async Task RespondeErrorAsync(HttpContext context, Exception ex)
        {          
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            // Just log exception details in a log file, you don't need send it to the client
            var errorMessage = new ErrorMessage(context.Response.StatusCode,
                                               "An unexpected error occur, please, try later.",
                                               ex.Message);

            await context.Response.WriteAsync(JsonConvert.SerializeObject(errorMessage));
            
        }
    }
}
