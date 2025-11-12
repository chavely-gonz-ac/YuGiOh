namespace YuGiOh.Domain.Models
{
    /// <summary>
    /// Represents a registration entry for a <see cref="Deck"/> in a specific <see cref="Tournament"/>.
    /// </summary>
    /// <remarks>
    /// Each registration indicates that a player (through their deck) is enrolled to compete
    /// in a particular tournament. It also tracks the player's state within that tournament,
    /// such as whether they were accepted, are currently playing, or have won.
    /// </remarks>
    public class Registration
    {
        /// <summary>
        /// Gets or sets the unique identifier of the registration.
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the registration
        /// has been accepted by the tournament organizers.
        /// </summary>
        /// <remarks>
        /// Registrations might require manual approval before participation is allowed.
        /// </remarks>
        /// <example>true</example>
        public bool? Accepted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the player
        /// associated with this registration won the tournament.
        /// </summary>
        /// <example>false</example>
        public bool IsWinner { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the player is currently
        /// participating in the tournament (e.g., in progress or active round).
        /// </summary>
        /// <example>true</example>
        public bool? IsPlaying { get; set; }

        /// <summary>
        /// Gets or sets an optional textual description or comment
        /// related to the registration.
        /// </summary>
        /// <remarks>
        /// This may include notes from organizers, special conditions, or player remarks.
        /// </remarks>
        /// <example>"Approved by admin after deck verification."</example>
        public string? Description { get; set; }

        // ---------------------------------------------------------------
        // Foreign Keys and Navigation Properties
        // ---------------------------------------------------------------

        /// <summary>
        /// Gets or sets the foreign key of the <see cref="Deck"/> used for this registration.
        /// </summary>
        /// <example>3</example>
        public int DeckId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Deck"/> associated with this registration.
        /// </summary>
        public Deck Deck { get; set; } = null!;

        /// <summary>
        /// Gets or sets the foreign key of the <see cref="Tournament"/> this registration belongs to.
        /// </summary>
        /// <example>5</example>
        public int TournamentId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Tournament"/> in which the deck is registered.
        /// </summary>
        public Tournament Tournament { get; set; } = null!;

        // ---------------------------------------------------------------
        // Metadata
        // ---------------------------------------------------------------

        /// <summary>
        /// Gets or sets the UTC date and time when the registration was created.
        /// </summary>
        /// <remarks>
        /// Automatically set when the registration record is inserted into the database.
        /// Must be earlier than the tournament’s registration deadline.
        /// </remarks>
        /// <example>2025-11-08T14:30:00Z</example>
        public DateTime CreatedAt { get; set; }
    }
}
