using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WebApplication1
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
            var exception = errorFeature.Error;

            // the IsTrusted() extension method doesn't exist and
            // you should implement your own as you may want to interpret it differently
            // i.e. based on the current principal
            var errorDetail = context.Request.IsTrusted()
                ? exception.Demystify().ToString()
                : "The instance value should be used to identify the problem when calling customer support";

            var problemDetails = new ProblemDetails
            {
                Title = "An unexpected error occurred!",
                Status = 500,
                Detail = errorDetail,
                Instance = $"urn:myorganization:error:{Guid.NewGuid()}"
            };

            // log the exception etc..

            context.Response.WriteJson(problemDetails, "application/problem+json");

            await _next(context);
        }
    }
}
