namespace YuGiOh.Domain.Services
{
    /// <summary>
    /// Defines authentication and token refresh operations.
    /// </summary>
    public interface IAuthenticationHandler
    {
        /// <summary>
        /// Authenticates a user based on their identifier and password,
        /// returning a new access token and refresh token if successful.
        /// </summary>
        /// <param name="identifier">
        /// The unique login identifier of the account (e.g., username or email).
        /// </param>
        /// <param name="password">The user's password.</param>
        /// <param name="ipAddress">
        /// The IP address from which the authentication request originated.
        /// </param>
        /// <returns>
        /// A tuple containing the generated <c>AccessToken</c> and <c>RefreshToken</c>.
        /// </returns>
        Task<(string AccessToken, string RefreshToken)> AuthenticateAsync(
            string identifier, string password, string ipAddress);

        /// <summary>
        /// Refreshes an existing access token using a valid refresh token.
        /// </summary>
        /// <param name="refreshToken">The refresh token used to generate a new pair of tokens.</param>
        /// <param name="ipAddress">The IP address from which the request originated.</param>
        /// <returns>
        /// A tuple containing a new <c>AccessToken</c> and <c>RefreshToken</c>.
        /// </returns>
        Task<(string AccessToken, string RefreshToken)> RefreshAsync(
            string refreshToken, string ipAddress);
    }
}
