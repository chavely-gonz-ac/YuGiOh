using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Ardalis.Specification;
using YuGiOh.Domain.Models;
using YuGiOh.Domain.Services;
using YuGiOh.Domain.Exceptions;
using System.Net;

namespace YuGiOh.Infrastructure.Identity.Services
{
    /// <summary>
    /// Provides functionality for generating, refreshing, and revoking
    /// authentication tokens for application accounts.
    /// </summary>
    public class AccountTokensProvider : IAccountTokensProvider
    {
        private readonly IRepositoryBase<RefreshTokenData> _refreshTokenDataRepository;
        private readonly UserManager<Account> _userManager;
        private readonly JWTOptions _jwtOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountTokensProvider"/> class.
        /// </summary>
        public AccountTokensProvider(
            UserManager<Account> userManager,
            IOptions<JWTOptions> jwtOptions,
            IRepositoryBase<RefreshTokenData> refreshTokenDataRepository)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _jwtOptions = jwtOptions?.Value ?? throw new ArgumentNullException(nameof(jwtOptions));
            _refreshTokenDataRepository = refreshTokenDataRepository ?? throw new ArgumentNullException(nameof(refreshTokenDataRepository));
        }

        /// <inheritdoc/>
        public async Task<string> GenerateJWTokenAsync(string accountId)
        {
            if (string.IsNullOrWhiteSpace(accountId))
                throw new APIException(HttpStatusCode.BadRequest, "Account ID cannot be null or empty.");

            var account = await _userManager.FindByIdAsync(accountId)
                ?? throw new APIException(HttpStatusCode.NotFound, $"No account found for Id: {accountId}");
            var relatedRoles = await _userManager.GetRolesAsync(account);

            var authClaims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, account.Id),
                new(JwtRegisteredClaimNames.Email, account.Email ?? string.Empty),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Add roles as separate claims for ASP.NET Core's role-based authorization
            foreach (var role in relatedRoles)
                authClaims.Add(new Claim(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                expires: DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes),
                claims: authClaims,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <inheritdoc/>
        public async Task<string> AddRefreshTokenAsync(string oldToken, string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(oldToken))
                throw new APIException(HttpStatusCode.BadRequest, "Refresh token cannot be empty.");

            if (string.IsNullOrWhiteSpace(ipAddress))
                throw new APIException(HttpStatusCode.BadRequest, "IP address is required.");

            var refreshData = await GetRefreshTokenDataAsync(oldToken);

            if (refreshData == null || !refreshData.IsActive)
                throw new APIException(HttpStatusCode.BadRequest, "Invalid or expired refresh token.");

            if (refreshData.CreatedByIp != ipAddress)
                throw new APIException(HttpStatusCode.Unauthorized, "Refresh token origin is invalid.");

            var accountId = refreshData.AccountId;
            await RevokeRefreshTokenAsync(refreshData, ipAddress);
            return await AddRefreshTokenDataAsync(accountId, ipAddress);

        }

        /// <inheritdoc/>
        public async Task<string> AddRefreshTokenByIdAsync(string accountId, string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(accountId))
                throw new APIException(HttpStatusCode.BadRequest, "Account ID is required.");

            if (string.IsNullOrWhiteSpace(ipAddress))
                throw new APIException(HttpStatusCode.BadRequest, "IP address is required.");

            var spec = new RefreshTokenByAccountIdSpec(accountId);
            var existingToken = await _refreshTokenDataRepository.FirstOrDefaultAsync(spec);

            if (existingToken is { IsActive: true })
                await RevokeRefreshTokenAsync(existingToken, ipAddress);
            else if (existingToken != null)
                await _refreshTokenDataRepository.DeleteAsync(existingToken);

            return await AddRefreshTokenDataAsync(accountId, ipAddress);
        }

        /// <inheritdoc/>
        public async Task RevokeRefreshTokenAsync(string token, string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new APIException(HttpStatusCode.BadRequest, "Refresh token cannot be empty.");

            var refreshData = await GetRefreshTokenDataAsync(token);
            if (refreshData == null || !refreshData.IsActive)
                throw new APIException(HttpStatusCode.BadRequest, "Invalid or expired refresh token.");
            await RevokeRefreshTokenAsync(refreshData, ipAddress);
        }

        protected async Task RevokeRefreshTokenAsync(RefreshTokenData refreshData, string ipAddress)
        {
            refreshData.Revoked = DateTime.UtcNow;
            refreshData.RevokedByIp = ipAddress;

            await _refreshTokenDataRepository.UpdateAsync(refreshData);
        }

        private async Task<string> AddRefreshTokenDataAsync(string accountId, string ipAddress)
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            var token = Convert.ToBase64String(randomBytes);

            var tokenData = new RefreshTokenData
            {
                Token = token,
                AccountId = accountId,
                Expires = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };

            await _refreshTokenDataRepository.AddAsync(tokenData);
            return token;
        }

        public async Task<RefreshTokenData?> GetRefreshTokenDataAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new APIException(HttpStatusCode.BadRequest, "Token cannot be empty.");
            return await _refreshTokenDataRepository.GetByIdAsync(token);
        }

        public sealed class RefreshTokenByAccountIdSpec : Specification<RefreshTokenData>
        {
            public RefreshTokenByAccountIdSpec(string accountId, bool onlyActive = true)
            {
                Query.Where(d => d.AccountId == accountId);

                if (onlyActive)
                    Query.Where(d => d.Revoked == null && d.Expires > DateTime.UtcNow);
            }
        }

    }
}
