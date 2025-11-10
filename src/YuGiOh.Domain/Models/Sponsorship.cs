namespace YuGiOh.Domain.Models
{
    /// <summary>
    /// Represents a sponsorship relationship between a sponsor and a tournament.
    /// </summary>
    /// <remarks>
    /// This entity defines a many-to-many association between
    /// <see cref="Sponsor"/> and <see cref="Tournament"/> with additional metadata such as creation date.
    /// </remarks>
    public class Sponsorship
    {
        /// <summary>
        /// Gets or sets the identifier of the sponsor providing support.
        /// </summary>
        /// <example>"sponsor-001"</example>
        public required string SponsorId { get; set; }

        /// <summary>
        /// Gets or sets the sponsor entity associated with this sponsorship.
        /// </summary>
        public Sponsor? Sponsor { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the sponsored tournament.
        /// </summary>
        /// <example>42</example>
        public required int TournamentId { get; set; }

        /// <summary>
        /// Gets or sets the tournament entity associated with this sponsorship.
        /// </summary>
        public Tournament? Tournament { get; set; }

        /// <summary>
        /// Gets or sets the UTC timestamp indicating when this sponsorship was created.
        /// </summary>
        /// <example>2025-11-10T12:00:00Z</example>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
