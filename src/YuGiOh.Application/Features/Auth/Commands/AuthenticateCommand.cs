using MediatR;
using YuGiOh.Domain.Services;

namespace YuGiOh.Application.Features.Auth.Commands
{
    /// <summary>
    /// Represents a command request for authenticating a user by their credentials.
    /// </summary>
    /// <remarks>
    /// This command is responsible for user login operations.  
    /// It accepts either an email or username as the handler and a password, 
    /// optionally including the IP address of the requester.
    /// </remarks>
    public class AuthenticateCommand : IRequest<(string AccessToken, string RefreshToken)>
    {
        /// <summary>
        /// Gets or sets the user’s identifying handler, which can be an email or username.
        /// </summary>
        /// <remarks>
        /// The authentication process will attempt to resolve the account based on this value.
        /// </remarks>
        public required string Handler { get; set; }

        /// <summary>
        /// Gets or sets the user's password for authentication.
        /// </summary>
        public required string Password { get; set; }

        /// <summary>
        /// Gets or sets the optional IP address from which the login request originated.
        /// </summary>
        /// <remarks>
        /// This value can be used for security auditing or login location validation.
        /// </remarks>
        public string? IpAddress { get; set; }
    }

    /// <summary>
    /// Handles the execution of an <see cref="AuthenticateCommand"/> by invoking
    /// the domain-level authentication logic through <see cref="IAuthenticationHandler"/>.
    /// </summary>
    /// <remarks>
    /// This class acts as the mediator layer between the application command pipeline
    /// and the underlying domain authentication service.
    /// </remarks>
    public class AuthenticateCommandHandler
        : IRequestHandler<AuthenticateCommand, (string AccessToken, string RefreshToken)>
    {
        private readonly IAuthenticationHandler _authenticationHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticateCommandHandler"/> class.
        /// </summary>
        /// <param name="authenticationHandler">The authentication service responsible for verifying credentials and issuing tokens.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="authenticationHandler"/> is null.</exception>
        public AuthenticateCommandHandler(IAuthenticationHandler authenticationHandler)
        {
            _authenticationHandler = authenticationHandler
                ?? throw new ArgumentNullException(nameof(authenticationHandler));
        }

        /// <summary>
        /// Handles the authentication process using the provided command data.
        /// </summary>
        /// <param name="request">The authentication command containing credentials and optional IP address.</param>
        /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
        /// <returns>
        /// A tuple containing the generated <c>AccessToken</c> and <c>RefreshToken</c> for the authenticated user.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="request"/> is null.</exception>
        public async Task<(string AccessToken, string RefreshToken)> Handle(
            AuthenticateCommand request,
            CancellationToken cancellationToken)
        {
            // Ensure the command is valid
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            // Delegate to the domain authentication handler
            return await _authenticationHandler.AuthenticateAsync(
                request.Handler,
                request.Password,
                request.IpAddress
            );
        }
    }
}
