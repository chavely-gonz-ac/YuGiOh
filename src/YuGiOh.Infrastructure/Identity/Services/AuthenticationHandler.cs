using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using YuGiOh.Domain.Services;
using YuGiOh.Domain.Exceptions;

namespace YuGiOh.Infrastructure.Identity.Services
{
    /// <summary>
    /// Handles user authentication and JWT token management.
    /// Provides secure login and refresh mechanisms with consistent APIException error handling.
    /// </summary>
    public class AuthenticationHandler : IAuthenticationHandler
    {
        private readonly IAccountTokensProvider _accountTokensProvider;
        private readonly UserManager<Account> _userManager;
        private readonly SignInManager<Account> _signInManager;
        private readonly JWTOptions _jwtOptions;
        private readonly ILogger<AuthenticationHandler> _logger;

        public AuthenticationHandler(
            IAccountTokensProvider accountTokensProvider,
            UserManager<Account> userManager,
            SignInManager<Account> signInManager,
            IOptions<JWTOptions> jwtOptions,
            ILogger<AuthenticationHandler> logger)
        {
            _accountTokensProvider = accountTokensProvider ?? throw new ArgumentNullException(nameof(accountTokensProvider));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _jwtOptions = jwtOptions?.Value ?? throw new ArgumentNullException(nameof(jwtOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        /// <summary>
        /// Authenticates a user and returns a pair of JWT access and refresh tokens.
        /// </summary>
        /// <param name="handler">Username or email of the user.</param>
        /// <param name="password">User's password.</param>
        /// <param name="ipAddress">Client IP address.</param>
        /// <returns>A tuple (AccessToken, RefreshToken).</returns>
        /// <exception cref="APIException">Thrown when input or authentication fails.</exception>
        public async Task<(string AccessToken, string RefreshToken)> AuthenticateAsync(string handler, string password, string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(handler))
                throw APIException.BadRequest("Username or email is required.");
            if (string.IsNullOrWhiteSpace(password))
                throw APIException.BadRequest("Password is required.");

            var account = await GetAccount(handler);

            var result = await _signInManager.CheckPasswordSignInAsync(account, password, lockoutOnFailure: true);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Failed authentication attempt for handler: {Handler}", handler);
                throw APIException.Unauthorized("Invalid credentials.");
            }

            try
            {
                var accessToken = await _accountTokensProvider.GenerateJWTokenAsync(account.Id);
                var refreshToken = await _accountTokensProvider.AddRefreshTokenByIdAsync(account.Id, ipAddress);

                _logger.LogInformation("User {UserId} authenticated successfully.", account.Id);

                return (accessToken, refreshToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating tokens for user {UserId}", account.Id);
                throw APIException.Internal("Failed to generate authentication tokens.", ex.Message, ex);
            }
        }

        /// <summary>
        /// Finds an account by username or email and ensures it's confirmed.
        /// </summary>
        /// <param name="handler">Username or email string.</param>
        /// <returns>The user account entity.</returns>
        /// <exception cref="APIException">Thrown when not found or not confirmed.</exception>
        private async Task<Account> GetAccount(string handler)
        {
            var account = (await _userManager.FindByNameAsync(handler)
                           ?? await _userManager.FindByEmailAsync(handler));

            if (account == null)
            {
                _logger.LogWarning("Account not found for handler: {Handler}", handler);
                throw APIException.Unauthorized("Invalid credentials.");
            }

            if (!await _userManager.IsEmailConfirmedAsync(account))
                throw APIException.Forbidden("Email is not confirmed.");

            // Future account state validation (optional)
            // if (account.Statement == Domain.Enums.AccountStatement.Deleted)
            //     throw APIException.Forbidden("Account has been deleted.");
            //
            // if (account.Statement == Domain.Enums.AccountStatement.Inactive)
            //     throw APIException.Forbidden("Account is inactive.");

            return account;
        }

        /// <inheritdoc/>
        /// <summary>
        /// Refreshes an existing token pair using a valid refresh token.
        /// </summary>
        /// <param name="refreshToken">Old refresh token.</param>
        /// <param name="ipAddress">Client IP address.</param>
        /// <returns>New (AccessToken, RefreshToken) pair.</returns>
        /// <exception cref="APIException">Thrown for invalid or expired refresh tokens.</exception>
        public async Task<(string AccessToken, string RefreshToken)> RefreshAsync(string refreshToken, string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw APIException.BadRequest("Refresh token is required.");
            if (string.IsNullOrWhiteSpace(ipAddress))
                throw APIException.BadRequest("IP address is required.");

            try
            {
                var newRefreshToken = await _accountTokensProvider.AddRefreshTokenAsync(refreshToken, ipAddress);
                var refreshData = await _accountTokensProvider.GetRefreshTokenDataAsync(newRefreshToken);

                if (refreshData == null || !refreshData.IsActive)
                    throw APIException.Unauthorized("Invalid or expired refresh token.");

                var accessToken = await _accountTokensProvider.GenerateJWTokenAsync(refreshData.AccountId);

                _logger.LogInformation("Access token refreshed for account {AccountId}", refreshData.AccountId);

                return (accessToken, newRefreshToken);
            }
            catch (APIException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing authentication tokens.");
                throw APIException.Internal("Failed to refresh authentication tokens.", ex.Message, ex);
            }
        }
    }
}
