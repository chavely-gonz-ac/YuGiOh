namespace YuGiOh.Domain.Models
{
    /// <summary>
    /// Represents a persisted refresh token associated with an account.
    /// Used to manage authentication sessions, handle token rotation,
    /// and track revocation for security auditing.
    /// </summary>
    public class RefreshTokenData
    {
        /// <summary>
        /// Gets or sets the unique token string.
        /// </summary>
        public required string Token { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the account that owns this token.
        /// </summary>
        public required string AccountId { get; set; }

        /// <summary>
        /// Gets or sets the UTC datetime when this token will expire.
        /// </summary>
        public DateTime Expires { get; set; }

        /// <summary>
        /// Gets or sets the UTC datetime when this token was created.
        /// </summary>
        public DateTime Created { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the IP address from which the token was created.
        /// </summary>
        public required string CreatedByIp { get; set; }

        /// <summary>
        /// Gets or sets the UTC datetime when this token was revoked, if applicable.
        /// </summary>
        public DateTime? Revoked { get; set; }

        /// <summary>
        /// Gets or sets the IP address from which the token was revoked.
        /// </summary>
        public string? RevokedByIp { get; set; }

        /// <summary>
        /// Gets or sets the replacement token string if this token was rotated.
        /// </summary>
        public string? ReplacedByToken { get; set; }

        /// <summary>
        /// Gets a value indicating whether the token has expired.
        /// </summary>
        public bool IsExpired => DateTime.UtcNow >= Expires;

        /// <summary>
        /// Gets a value indicating whether the token is still active (not expired or revoked).
        /// </summary>
        public bool IsActive => Revoked == null && !IsExpired;

        /// <summary>
        /// Returns the string representation of the token (for debugging).
        /// </summary>
        public override string ToString() => $"{Token} (Active: {IsActive})";
    }
}
