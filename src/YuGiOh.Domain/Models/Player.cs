namespace YuGiOh.Domain.Models
{
    /// <summary>
    /// Represents a player in the Yu-Gi-Oh! system.
    /// Each player corresponds to an account in the identity system,
    /// but this entity remains domain-pure and unaware of Identity.
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Gets or sets the unique identifier of the player.
        /// This value corresponds to the <c>Account.Id</c> from the identity layer.
        /// </summary>
        public required string Id { get; set; }

        /// <summary>
        /// Gets or sets the foreign key to the player's address.
        /// </summary>
        public int AddressId { get; set; }

        /// <summary>
        /// Gets or sets the player's address information.
        /// </summary>
        public Address? Address { get; set; }

        /// <summary>
        /// Collection of Decks associated with this Player.
        /// </summary>
        /// <remarks>
        /// One Player can have many Decks. This represents the one-to-many relationship.
        /// </remarks>
        public ICollection<Deck> Decks { get; set; } = new List<Deck>();

        /// <summary>
        /// Returns the string representation of the player (its ID).
        /// </summary>
        public override string ToString() => Id;
    }
}
