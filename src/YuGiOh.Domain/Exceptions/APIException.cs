using System.Net;

namespace YuGiOh.Domain.Exceptions
{
    /// <summary>
    /// Represents an error that occurs during the execution of an API request.
    /// Provides a standardized structure for exception handling and responses.
    /// </summary>
    public class APIException : Exception
    {
        /// <summary>
        /// Gets the HTTP status code associated with this exception.
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Gets a short, human-readable title describing the type of error.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets an optional additional detail message that may assist in debugging.
        /// </summary>
        public string? Detail { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="APIException"/> class.
        /// </summary>
        /// <param name="statusCode">The HTTP status code to return (default: 500).</param>
        /// <param name="message">The main message describing the error.</param>
        /// <param name="title">A short title for the error (default: "API Error").</param>
        /// <param name="detail">Optional additional detail message.</param>
        /// <param name="innerException">Optional inner exception for debugging.</param>
        public APIException(
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
            string message = "An unexpected error occurred.",
            string title = "API Error",
            string? detail = null,
            Exception? innerException = null
        ) : base(message, innerException)
        {
            StatusCode = statusCode;
            Title = title;
            Detail = detail;
        }

        /// <summary>
        /// Converts this exception into a serializable object that can be returned by the API layer.
        /// Keeps it decoupled from ASP.NET MVC or other frameworks.
        /// </summary>
        /// <param name="includeInnerException">Whether to include inner exception details.</param>
        public object ToSerializableObject(bool includeInnerException = false)
        {
            var obj = new Dictionary<string, object?>
            {
                ["status"] = (int)StatusCode,
                ["title"] = Title,
                ["message"] = Message,
                ["detail"] = Detail,
                ["timestamp"] = DateTime.UtcNow
            };

            if (includeInnerException && InnerException is not null)
                obj["innerException"] = InnerException.Message;

            return obj;
        }

        // === Factory methods ===

        public static APIException BadRequest(string message, string? detail = null, Exception? innerException = null)
            => new(HttpStatusCode.BadRequest, message, "Bad Request", detail, innerException);

        public static APIException NotFound(string message, string? detail = null, Exception? innerException = null)
            => new(HttpStatusCode.NotFound, message, "Not Found", detail, innerException);

        public static APIException Unauthorized(string message = "Unauthorized access.", string? detail = null, Exception? innerException = null)
            => new(HttpStatusCode.Unauthorized, message, "Unauthorized", detail, innerException);

        public static APIException Forbidden(string message = "Access forbidden.", string? detail = null, Exception? innerException = null)
            => new(HttpStatusCode.Forbidden, message, "Forbidden", detail, innerException);

        public static APIException Internal(string message = "An internal server error occurred.", string? detail = null, Exception? innerException = null)
            => new(HttpStatusCode.InternalServerError, message, "Internal Server Error", detail, innerException);
    }
}
