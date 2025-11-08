using System.ComponentModel.DataAnnotations;

namespace YuGiOh.Domain.Models
{
    /// <summary>
    /// Represents a Yu-Gi-Oh! Archetype, which groups related cards.
    /// </summary>
    public class Archetype
    {
        /// <summary>
        /// The unique identifier of the Archetype.
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// The name of the Archetype.
        /// </summary>
        /// <remarks>Examples: "Blue-Eyes", "Dark Magician"</remarks>
        /// <example>Blue-Eyes</example>
        [Required]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public required string Name { get; set; }

        /// <summary>
        /// Collection of Decks associated with this Archetype.
        /// </summary>
        /// <remarks>
        /// One Archetype can have many Decks. This represents the one-to-many relationship.
        /// </remarks>
        public ICollection<Deck> Decks { get; set; } = new List<Deck>();
    }
}
