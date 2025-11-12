namespace YuGiOh.Domain.Models
{
    /// <summary>
    /// Represents a Yu-Gi-Oh! tournament where players compete using their decks.
    /// </summary>
    /// <remarks>
    /// A tournament consists of one or more rounds, involves player registrations,
    /// and may include sponsors. It has a start date and a registration deadline,
    /// after which no more participants can join.
    /// </remarks>
    public class Tournament
    {
        /// <summary>
        /// Gets or sets the unique identifier of the tournament.
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the tournament.
        /// </summary>
        /// <example>World Championship Qualifier 2025</example>
        public required string Name { get; set; }

        /// <summary>
        /// Gets or sets the foreign key of the address where the tournament takes place.
        /// </summary>
        public int AddressId { get; set; }

        /// <summary>
        /// Gets or sets the physical address where the tournament is held.
        /// </summary>
        public Address? Address { get; set; }

        /// <summary>
        /// Gets or sets the UTC date and time when the tournament starts.
        /// </summary>
        /// <example>2025-12-01T09:00:00Z</example>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the UTC date and time representing the registration deadline.
        /// </summary>
        /// <remarks>
        /// No participants can register after this date.
        /// </remarks>
        /// <example>2025-11-28T23:59:59Z</example>
        public DateTime RegistrationLimit { get; set; }

        /// <summary>
        /// Gets or sets the collection of sponsorships linked to this tournament.
        /// </summary>
        /// <remarks>
        /// Each sponsorship associates a <see cref="Sponsor"/> with the current tournament.
        /// </remarks>
        public ICollection<Sponsorship> SponsoredBy { get; set; } = new List<Sponsorship>();
        public ICollection<Registration> Registrations { get; set; } = new List<Registration>();

        /// <summary>
        /// Gets or sets the UTC timestamp when the tournament was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Indicates whether the registration period is still open.
        /// </summary>
        public bool IsOpenRegistration => DateTime.UtcNow <= RegistrationLimit;

        /// <summary>
        /// Indicates whether the tournament has already started.
        /// </summary>
        public bool IsStarted => DateTime.UtcNow >= StartDate;
    }
}
