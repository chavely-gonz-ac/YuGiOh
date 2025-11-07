using MediatR;
using YuGiOh.Domain.Services;

namespace YuGiOh.Application.Features.Auth.Commands
{
    /// <summary>
    /// Represents a command request to refresh a user's authentication tokens.
    /// </summary>
    /// <remarks>
    /// This command is used when a user provides a valid refresh token
    /// to obtain a new access token and refresh token pair.
    /// It follows the CQRS pattern through the MediatR library.
    /// </remarks>
    public class RefreshTokenCommand : IRequest<(string AccessToken, string RefreshToken)>
    {
        /// <summary>
        /// Gets or sets the refresh token used to validate the request
        /// and issue a new pair of authentication tokens.
        /// </summary>
        public required string RefreshToken { get; set; }

        /// <summary>
        /// Gets or sets the IP address from which the token refresh request originated.
        /// </summary>
        /// <remarks>
        /// This value may be used for security logging or to verify token validity
        /// against the issuing device or session.
        /// </remarks>
        public required string IpAddress { get; set; }
    }

    /// <summary>
    /// Handles the execution of a <see cref="RefreshTokenCommand"/> by delegating the
    /// token refresh process to an <see cref="IAuthenticationHandler"/> implementation.
    /// </summary>
    /// <remarks>
    /// This handler forms part of the Application layer and mediates between the
    /// presentation tier (API/controllers) and the domain authentication logic.
    /// </remarks>
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, (string AccessToken, string RefreshToken)>
    {
        private readonly IAuthenticationHandler _authenticationHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshTokenCommandHandler"/> class.
        /// </summary>
        /// <param name="authenticationHandler">The authentication service responsible for refreshing tokens.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="authenticationHandler"/> is null.</exception>
        public RefreshTokenCommandHandler(IAuthenticationHandler authenticationHandler)
        {
            _authenticationHandler = authenticationHandler
                ?? throw new ArgumentNullException(nameof(authenticationHandler));
        }

        /// <summary>
        /// Handles the refresh token request by invoking domain logic to generate new tokens.
        /// </summary>
        /// <param name="request">The refresh token command containing the current token and client IP.</param>
        /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
        /// <returns>
        /// A tuple containing the new <c>AccessToken</c> and <c>RefreshToken</c> strings.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="request"/> is null.</exception>
        public async Task<(string AccessToken, string RefreshToken)> Handle(
            RefreshTokenCommand request,
            CancellationToken cancellationToken)
        {
            // Validate request
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            // Delegate to the domain authentication service to handle refresh logic
            return await _authenticationHandler.RefreshAsync(request.RefreshToken, request.IpAddress);
        }
    }
}
