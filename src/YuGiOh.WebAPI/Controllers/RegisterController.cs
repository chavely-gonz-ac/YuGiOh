using MediatR;
using Microsoft.AspNetCore.Mvc;
using YuGiOh.Application.Features.Auth.Queries;
using YuGiOh.Application.Features.Auth.Commands;
using YuGiOh.Domain.DTOs;

namespace YuGiOh.WebAPI.Controllers
{
    /// <summary>
    /// Provides endpoints for user registration and email confirmation operations.
    /// </summary>
    /// <remarks>
    /// This controller handles the registration workflow, including:
    /// <list type="number">
    /// <item>Receiving registration data and creating a new account.</item>
    /// <item>Sending an email confirmation link to the registered user.</item>
    /// <item>Validating and confirming the user’s email address.</item>
    /// </list>
    /// </remarks>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class RegisterController : APIControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterController"/> class.
        /// </summary>
        /// <param name="mediator">The MediatR instance used for dispatching registration and email commands.</param>
        public RegisterController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// Registers a new user and sends them an email confirmation link.
        /// </summary>
        /// <param name="request">The registration data containing user details and credentials.</param>
        /// <returns>
        /// Returns a success message if registration and email sending succeed,
        /// or an appropriate error response if a failure occurs.
        /// </returns>
        /// <response code="200">Registration completed successfully, awaiting email confirmation.</response>
        /// <response code="400">Returned when the registration request is invalid.</response>
        /// <response code="500">Returned when an unexpected server error occurs during registration.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestData request)
        {
            if (request == null)
                return BadRequest("Invalid registration request.");

            try
            {
                // Dispatch the registration command via MediatR
                var token = await Sender.Send(new RegisterCommand { Data = request });

                // Build the callback URL for email confirmation
                var callbackUrl = Url.Action(
                    nameof(ConfirmEmail),
                    "Register",
                    new { email = request.Email, token },
                    Request.Scheme);

                // Dispatch the command to send the confirmation email
                await Sender.Send(new SendConfirmationEmailCommand
                {
                    Email = request.Email,
                    CallbackURL = callbackUrl!
                });

                return Ok(new { Message = "Registration successful. Please check your email to confirm your account." });
            }
            catch (Exception ex)
            {
                // Log for diagnostics (could be integrated with structured logging middleware)
                Console.WriteLine($"[Register Error] {ex.Message}");
                return StatusCode(500, "An error occurred during registration. Please try again later.");
            }
        }

        /// <summary>
        /// Confirms a user's email address using the verification link sent during registration.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="token">The confirmation token sent via email.</param>
        /// <returns>
        /// Returns a simple HTML confirmation page indicating success or failure.
        /// </returns>
        /// <response code="200">Displays a confirmation message.</response>
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            {
                return Content(BuildHtmlPage("Invalid email or token.", "http://localhost:3000/login"), "text/html");
            }

            try
            {
                var command = new ConfirmEmailQuery { Email = email, Token = token };
                var result = await Sender.Send(command);

                if (result)
                    return Content(BuildHtmlPage("Email confirmed successfully!", "http://localhost:3000/login"), "text/html");

                return Content(BuildHtmlPage("Invalid or expired confirmation link.", "http://localhost:3000/login"), "text/html");
            }
            catch (Exception)
            {
                return Content(BuildHtmlPage("An error occurred while confirming your email.", "http://localhost:3000/login"), "text/html");
            }
        }

        /// <summary>
        /// Builds a simple styled HTML page for displaying confirmation results to the user.
        /// </summary>
        /// <param name="message">The message to display to the user.</param>
        /// <param name="loginUrl">The URL to redirect the user to the login page.</param>
        /// <returns>An HTML string representing the page.</returns>
        private string BuildHtmlPage(string message, string loginUrl)
        {
            return $@"
            <!DOCTYPE html>
            <html lang='en'>
            <head>
                <meta charset='UTF-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <title>Email Confirmation</title>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        background: linear-gradient(135deg, #e0bbff, #d1a3ff);
                        display: flex;
                        justify-content: center;
                        align-items: center;
                        height: 100vh;
                        margin: 0;
                    }}
                    .card {{
                        background-color: #fff;
                        padding: 2rem 3rem;
                        border-radius: 12px;
                        text-align: center;
                        box-shadow: 0 8px 16px rgba(0,0,0,0.2);
                    }}
                    h1 {{
                        color: #4b0082;
                        margin-bottom: 1rem;
                    }}
                    a {{
                        display: inline-block;
                        margin-top: 1rem;
                        padding: 0.75rem 1.5rem;
                        background-color: #8a2be2;
                        color: #fff;
                        text-decoration: none;
                        border-radius: 6px;
                        font-weight: bold;
                        transition: background-color 0.3s;
                    }}
                    a:hover {{
                        background-color: #7a1ecf;
                    }}
                </style>
            </head>
            <body>
                <div class='card'>
                    <h1>{message}</h1>
                    <a href='{loginUrl}'>Go to Login</a>
                </div>
            </body>
            </html>";
        }
    }
}
