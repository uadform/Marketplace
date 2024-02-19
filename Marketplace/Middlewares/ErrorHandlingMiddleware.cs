using Contracts.Exceptions;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Marketplace.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                switch (ex)
                {
                    case NotFoundException e:
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    case UnprocessableEntityException e:
                        response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                        break;
                    case PaymentRequiredException e:
                        response.StatusCode = (int)HttpStatusCode.PaymentRequired;
                        break;
                    default:
                        _logger.LogError(ex, "Unhandled exception occurred.");
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                var result = JsonSerializer.Serialize(new { message = ex.Message });
                await response.WriteAsync(result);
            }
        }
    }
}
