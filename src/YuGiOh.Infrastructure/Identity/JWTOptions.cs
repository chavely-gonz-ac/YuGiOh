using System.ComponentModel.DataAnnotations;

namespace YuGiOh.Infrastructure.Identity
{
    /// <summary>
    /// Represents configuration settings for JSON Web Token (JWT) authentication.
    /// </summary>
    /// <remarks>
    /// These values should be loaded from configuration (e.g. <c>appsettings.json</c>)
    /// under the <c>JWT</c> section and validated.
    /// </remarks>
    public class JWTOptions
    {
        /// <summary>
        /// Secret key used to sign and validate JWT tokens.
        /// Must be at least 32 characters long for HMAC-SHA256 security.
        /// </summary>
        [Required]
        [MinLength(32, ErrorMessage = "SecretKey should be at least 32 characters for security.")]
        public required string SecretKey { get; set; }

        /// <summary>
        /// Token issuer — typically application name or domain.
        /// </summary>
        [Required]
        public required string Issuer { get; set; }

        /// <summary>
        /// Token audience — identifies valid token consumers (e.g. client apps).
        /// </summary>
        [Required]
        public required string Audience { get; set; }

        /// <summary>
        /// Duration in minutes before an access token expires.
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "AccessTokenExpirationMinutes must be greater than 0.")]
        public int AccessTokenExpirationMinutes { get; set; }

        /// <summary>
        /// Duration in days before a refresh token expires.
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "RefreshTokenExpirationDays must be greater than 0.")]
        public int RefreshTokenExpirationDays { get; set; }
    }
}
