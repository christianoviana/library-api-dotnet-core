using AspNetCoreRateLimit;
using Library.API.Domain.Results.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Library.API.Middleware.Exceptions
{
    public class GlobalIpRateLimitMiddleware : IpRateLimitMiddleware
    {
        public GlobalIpRateLimitMiddleware(RequestDelegate next, IOptions<IpRateLimitOptions> options, IRateLimitCounterStore counterStore, IIpPolicyStore policyStore, IRateLimitConfiguration config, ILogger<IpRateLimitMiddleware> logger) : base(next, options, counterStore, policyStore, config, logger)
        {
        }

        public override Task ReturnQuotaExceededResponse(HttpContext httpContext, RateLimitRule rule, string retryAfter)
        {
            var retryTime = DateTime.Now.AddSeconds(Math.Ceiling(Convert.ToDouble(retryAfter)));

            string message = $"You exceed the limit of {rule.Limit} in period {rule.Period}";
            string detail = $"Retry after {retryTime}";

            httpContext.Response.Headers.Add(new KeyValuePair<string, StringValues>("X-Rate-Limit-Reset", retryTime.ToString()));

            ErrorMessage error = new ErrorMessage((int)HttpStatusCode.TooManyRequests, message, detail);
            httpContext.Response.StatusCode = (int) HttpStatusCode.TooManyRequests;

            return httpContext.Response.WriteAsync(JsonConvert.SerializeObject(error));
        }
    }
}
