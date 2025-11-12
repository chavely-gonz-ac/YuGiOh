/* 
 * src/Domain/Models/Deck.cs
 * 
 * Represents a Yu-Gi-Oh! Deck.
 * A Deck is composed of cards and belongs to a Player and optionally to an Archetype.
 * This model is annotated for Swagger/OpenAPI documentation.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YuGiOh.Domain.Models
{
    /// <summary>
    /// Represents a Yu-Gi-Oh! Deck.
    /// </summary>
    public class Deck
    {
        /// <summary>
        /// The unique identifier of the Deck.
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// The name of the Deck.
        /// </summary>
        /// <example>Blue-Eyes Control</example>
        [Required]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public required string Name { get; set; }

        /// <summary>
        /// Number of cards in the main deck.
        /// </summary>
        /// <example>40</example>
        [Range(0, int.MaxValue)]
        public int MainDeckSize { get; set; }

        /// <summary>
        /// Number of cards in the side deck.
        /// </summary>
        /// <example>15</example>
        [Range(0, int.MaxValue)]
        public int SideDeckSize { get; set; }

        /// <summary>
        /// Number of cards in the extra deck.
        /// </summary>
        /// <example>15</example>
        [Range(0, int.MaxValue)]
        public int ExtraDeckSize { get; set; }

        /// <summary>
        /// ID of the Player who owns the Deck.
        /// </summary>
        /// <example>user-123</example>
        [Required]
        public required string OwnerId { get; set; }

        /// <summary>
        /// Navigation property for the Deck's owner.
        /// </summary>
        public Player? Owner { get; set; }

        /// <summary>
        /// ID of the Archetype associated with the Deck.
        /// </summary>
        /// <example>1</example>
        public int ArchetypeId { get; set; }

        /// <summary>
        /// Navigation property for the Deck's Archetype.
        /// </summary>
        public Archetype? Archetype { get; set; }
        public ICollection<Registration> Registrations { get; set; } = new List<Registration>();

        /// <summary>
        /// Optional collection of Registrations associated with the Deck.
        /// Uncomment if needed.
        /// </summary>
        // public IEnumerable<Registration> Registrations { get; set; }

        /// <summary>
        /// The date and time when the Deck was created.
        /// </summary>
        /// <example>2025-11-08T14:30:00Z</example>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
