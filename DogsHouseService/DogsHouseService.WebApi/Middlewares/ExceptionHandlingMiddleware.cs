using System.Net;
using System.Text.Json;

namespace DogsHouseService.WebApi.Middlewares
{
    /// <summary>
    /// Middleware for handling exceptions globally
    /// </summary>
    /// <param name="next">The next delegate.</param>
    /// <param name="logger">The logger.</param>
    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        private readonly RequestDelegate next = next ?? throw new ArgumentNullException(nameof(next));
        private readonly ILogger<ExceptionHandlingMiddleware> logger = logger ?? throw new ArgumentNullException(nameof(logger));

        /// <summary>
        /// Invokes the middleware to handle exceptions
        /// </summary>
        /// <param name="context">The http context.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, logger);
            }
        }

        /// <summary>
        /// Handles the exception and writes the response
        /// </summary>
        /// <param name="context">The http context.</param>
        /// <param name="exception">The caught exception.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private static async Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger logger)
        {
            HttpStatusCode status;
            string message;

            switch (exception)
            {
                case ArgumentNullException:
                case ArgumentException:
                    status = HttpStatusCode.BadRequest;
                    message = exception.Message;
                    break;

                case KeyNotFoundException:
                    status = HttpStatusCode.NotFound;
                    message = exception.Message;
                    break;

                case InvalidOperationException:
                    status = HttpStatusCode.Conflict;
                    message = exception.Message;
                    break;

                default:
                    status = HttpStatusCode.InternalServerError;
                    message = "An unexpected error occurred.";
                    break;
            }

            logger.LogError(exception, "Unhandled exception caught by middleware.");

            var response = new
            {
                error = message,
                statusCode = (int)status
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}
