namespace YuGiOh.Domain.Models
{
    /// <summary>
    /// Represents a type of street used in address information,
    /// such as "Street", "Avenue", or "Boulevard".
    /// </summary>
    public class StreetType
    {
        /// <summary>
        /// Gets or sets the unique identifier for the street type.
        /// </summary>
        public int Id { get; set; }

        public required string Language { get; set; }

        /// <summary>
        /// Gets or sets the name of the street type.  
        /// Example values: "Street", "Avenue", "Road", "Boulevard".
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Returns the string representation of the street type (its name).
        /// </summary>
        public override string ToString() => Name;
    }
}
