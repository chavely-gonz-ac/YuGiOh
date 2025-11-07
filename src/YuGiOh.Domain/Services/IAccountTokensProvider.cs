using YuGiOh.Domain.Models;

namespace YuGiOh.Domain.Services
{
    /// <summary>
    /// Provides functionality for generating, storing, and managing authentication tokens.
    /// Includes operations for JWT and refresh token management.
    /// </summary>
    public interface IAccountTokensProvider
    {
        /// <summary>
        /// Generates a JSON Web Token (JWT) for the specified account.
        /// </summary>
        /// <param name="accountId">The unique identifier of the account.</param>
        /// <returns>A signed JWT as a string.</returns>
        Task<string> GenerateJWTokenAsync(string accountId);

        /// <summary>
        /// Creates and stores a new refresh token associated with the specified account ID and IP address.
        /// </summary>
        /// <param name="accountId">The unique identifier of the account.</param>
        /// <param name="ipAddress">The IP address from which the token was generated.</param>
        /// <returns>The newly created refresh token string.</returns>
        Task<string> AddRefreshTokenByIdAsync(string accountId, string ipAddress);

        /// <summary>
        /// Creates a new refresh token by rotating (replacing) an existing one.
        /// </summary>
        /// <param name="oldToken">The old refresh token being replaced.</param>
        /// <param name="ipAddress">The IP address from which the request originated.</param>
        /// <returns>The new refresh token string.</returns>
        Task<string> AddRefreshTokenAsync(string oldToken, string ipAddress);

        /// <summary>
        /// Revokes a specific refresh token, preventing further use.
        /// </summary>
        /// <param name="token">The refresh token to revoke.</param>
        /// <param name="ipAddress">The IP address that performed the revocation.</param>
        Task RevokeRefreshTokenAsync(string token, string ipAddress);

        /// <summary>
        /// Retrieves the refresh token metadata and validation information by token value.
        /// </summary>
        /// <param name="token">The token to search for.</param>
        /// <returns>The refresh token data if found; otherwise, <c>null</c>.</returns>
        Task<RefreshTokenData?> GetRefreshTokenDataAsync(string token);
    }
}
