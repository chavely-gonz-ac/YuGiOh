using System.Net;
using System.Text.Json;
using YuGiOh.Domain.Exceptions;

namespace YuGiOh.WebAPI.Middlewares
{
    /// <summary>
    /// Middleware responsible for centralized exception handling across the Web API.
    /// </summary>
    /// <remarks>
    /// This middleware ensures that all unhandled exceptions are caught and translated into
    /// standardized JSON responses. It supports both <see cref="APIException"/> for expected
    /// domain or application-level errors, and generic <see cref="Exception"/> for unexpected ones.
    /// </remarks>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionHandlingMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the HTTP request pipeline.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="next"/> is null.</exception>
        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        /// <summary>
        /// Invokes the middleware for the current HTTP request context.
        /// </summary>
        /// <param name="context">The current <see cref="HttpContext"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method wraps the downstream request execution in a try/catch block and handles:
        /// <list type="bullet">
        /// <item>
        /// <description><see cref="APIException"/> — Translated into structured error responses with proper status codes.</description>
        /// </item>
        /// <item>
        /// <description>Unhandled <see cref="Exception"/> — Returns a 500 Internal Server Error with a generic message.</description>
        /// </item>
        /// </list>
        /// </remarks>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Continue to the next middleware or request handler
                await _next(context);
            }
            catch (APIException ex)
            {
                // Handle known API exceptions in a standardized format
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)ex.StatusCode;

                var responseBody = JsonSerializer.Serialize(ex.ToSerializableObject());
                await context.Response.WriteAsync(responseBody);
            }
            catch (Exception ex)
            {
                // ✅ Prevent crash: headers can’t be modified after response has started
                if (context.Response.HasStarted)
                {
                    Console.WriteLine("⚠️ Cannot modify response, it has already started.");
                    return;
                }

                // Clear any partially written content
                context.Response.Clear();
                // Handle unexpected, non-API exceptions (internal errors)
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var responseBody = JsonSerializer.Serialize(new
                {
                    status = 500,
                    title = "Internal Server Error",
                    message = ex.Message,
                    timestamp = DateTime.UtcNow
                });

                await context.Response.WriteAsync(responseBody);
            }
        }
    }
}

/*
    To enable this middleware, register it in the request pipeline:

    app.UseMiddleware<ExceptionHandlingMiddleware>();

    This should be placed early in the pipeline (right after `UseRouting()` 
    and before other middleware that may throw exceptions).
*/
