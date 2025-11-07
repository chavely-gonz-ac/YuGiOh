namespace YuGiOh.Domain.Models
{
    /// <summary>
    /// Represents a physical address used by players in the Yu-Gi-Oh! system.
    /// </summary>
    public class Address
    {
        /// <summary>
        /// Gets or sets the unique identifier for the address.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the ISO2 country code (e.g., "US", "MX", "ES").
        /// </summary>
        public required string CountryIso2 { get; set; }

        /// <summary>
        /// Gets or sets the ISO2 state code (optional).
        /// </summary>
        public string? StateIso2 { get; set; }

        /// <summary>
        /// Gets or sets the city name (optional).
        /// </summary>
        public string? City { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for the street type (optional).
        /// </summary>
        public int? StreetTypeId { get; set; }

        /// <summary>
        /// Gets or sets the street type (e.g., "Street", "Avenue", etc.).
        /// </summary>
        public StreetType? StreetType { get; set; }

        /// <summary>
        /// Gets or sets the name of the street (optional).
        /// </summary>
        public string? StreetName { get; set; }

        /// <summary>
        /// Gets or sets the building name or number (optional).
        /// </summary>
        public string? Building { get; set; }

        /// <summary>
        /// Gets or sets the apartment or unit identifier (optional).
        /// </summary>
        public string? Apartment { get; set; }

        /// <summary>
        /// Gets or sets the players associated with this address.
        /// </summary>
        public ICollection<Player> Players { get; set; } = new List<Player>();

        /// <summary>
        /// Returns a readable representation of the address.
        /// </summary>
        public override string ToString()
        {
            var parts = new List<string?> { StreetName, Building, Apartment, City, StateIso2, CountryIso2 };
            return string.Join(", ", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
        }
    }
}
