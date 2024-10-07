using System.Net;
using System.Text.Json;

namespace EventMaster.Server.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            context.Response.StatusCode = exception switch
            {
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                ArgumentNullException => (int)HttpStatusCode.BadRequest,
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                _ => (int)HttpStatusCode.InternalServerError,
            };
            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = GetDefaultMessageForStatusCode(context.Response.StatusCode),
                Detailed = exception.Message // для дебага
            };

            string jsonResponse = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(jsonResponse);
        }

        private static string GetDefaultMessageForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                (int)HttpStatusCode.Unauthorized => "Unauthorized access.",
                (int)HttpStatusCode.BadRequest => "Bad request. Please verify your inputs.",
                (int)HttpStatusCode.NotFound => "Resource not found.",
                (int)HttpStatusCode.InternalServerError => "Internal Server Error. Please try again later.",
                _ => "An unexpected error occurred."
            };
        }
    }
}