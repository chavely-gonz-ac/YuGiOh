using Microsoft.AspNetCore.Identity;

namespace YuGiOh.Infrastructure.Identity
{
    /// <summary>
    /// Represents an application user account managed by ASP.NET Core Identity.
    /// Extends the default <see cref="IdentityUser"/> with additional metadata.
    /// </summary>
    public class Account : IdentityUser
    {
        /// <summary>
        /// Gets or sets the UTC timestamp when the account was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Initializes a new instance of the <see cref="Account"/> class.
        /// </summary>
        public Account() : base() { }
    }
}
