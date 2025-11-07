using MediatR;
using Microsoft.AspNetCore.Mvc;
using YuGiOh.Application.Features.Auth.Commands;

namespace YuGiOh.WebAPI.Controllers
{
    /// <summary>
    /// Provides endpoints for user authentication, including login and token refresh operations.
    /// </summary>
    /// <remarks>
    /// This controller handles authentication-related actions such as issuing access and refresh tokens,
    /// as well as renewing tokens securely via HTTP-only cookies.
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : APIControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationController"/> class.
        /// </summary>
        /// <param name="mediator">The MediatR instance used for dispatching authentication commands.</param>
        public AuthenticationController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// Authenticates a user and issues a new access token and refresh token.
        /// </summary>
        /// <param name="request">The authentication command containing credentials and optional IP address.</param>
        /// <returns>
        /// A JSON object containing the access token, while storing the refresh token in a secure HttpOnly cookie.
        /// </returns>
        /// <response code="200">Returns the generated access token.</response>
        /// <response code="400">Returned when the request data is invalid or missing credentials.</response>
        /// <response code="401">Returned when authentication fails due to invalid credentials.</response>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthenticateCommand request)
        {
            // Basic request validation
            if (request == null)
                return BadRequest("Invalid request data.");

            if (string.IsNullOrWhiteSpace(request.Handler) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Handler and password are required.");

            // Auto-fill the request IP address if not provided by the client
            if (string.IsNullOrWhiteSpace(request.IpAddress))
            {
                request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            }

            try
            {
                // Execute the authentication command through MediatR
                var tokens = await Sender.Send(request);

                // Store refresh token securely in an HTTP-only cookie
                Response.Cookies.Append("refreshToken", tokens.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,           // inaccessible to JavaScript
                    Secure = true,             // requires HTTPS
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(7) // should match backend refresh token lifetime
                });

                // Return the access token to the client (refresh token remains in the cookie)
                return Ok(new
                {
                    AccessToken = tokens.AccessToken
                });
            }
            catch (Exception ex)
            {
                // Return a sanitized unauthorized response (avoid exposing internal error details)
                return Unauthorized(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Refreshes the user's access token using the refresh token stored in a secure cookie.
        /// </summary>
        /// <returns>
        /// A JSON object containing a new access token and an updated refresh token (rotated in the cookie).
        /// </returns>
        /// <response code="200">Returns the new access token and rotates the refresh token cookie.</response>
        /// <response code="400">Returned when the refresh token cookie is missing or invalid.</response>
        /// <response code="401">Returned when the refresh token is expired or invalid.</response>
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            // Retrieve the refresh token from the secure cookie
            var refreshToken = Request.Cookies["refreshToken"];
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            if (string.IsNullOrWhiteSpace(refreshToken))
                return BadRequest("Refresh token cookie missing.");

            try
            {
                // Execute the refresh command via MediatR
                var tokens = await Sender.Send(new RefreshTokenCommand
                {
                    RefreshToken = refreshToken,
                    IpAddress = ipAddress
                });

                // Rotate the refresh token by setting a new cookie
                Response.Cookies.Append("refreshToken", tokens.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                return Ok(new
                {
                    tokens.AccessToken
                });
            }
            catch (Exception ex)
            {
                // Return unauthorized with safe message
                return Unauthorized(new { Message = ex.Message });
            }
        }
    }
}
