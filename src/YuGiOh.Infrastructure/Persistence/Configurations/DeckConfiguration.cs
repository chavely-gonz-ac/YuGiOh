/* 
 * src/Infrastructure/Configurations/DeckConfiguration.cs
 * 
 * Configures the Deck entity for EF Core using Fluent API.
 * Defines keys, relationships, constraints, and default values.
 */

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YuGiOh.Domain.Models;

namespace YuGiOh.Infrastructure.Configurations
{
    /// <summary>
    /// EF Core configuration for the Deck entity.
    /// Maps properties, sets constraints, and defines relationships.
    /// </summary>
    public class DeckConfiguration : IEntityTypeConfiguration<Deck>
    {
        public void Configure(EntityTypeBuilder<Deck> builder)
        {
            // Set the table name
            builder.ToTable("Decks");

            // Primary key
            builder.HasKey(d => d.Id);

            // Name: required, max length 100
            builder.Property(d => d.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            // MainDeckSize: required, must be non-negative
            builder.Property(d => d.MainDeckSize)
                   .IsRequired();

            // SideDeckSize: required, must be non-negative
            builder.Property(d => d.SideDeckSize)
                   .IsRequired();

            // ExtraDeckSize: required, must be non-negative
            builder.Property(d => d.ExtraDeckSize)
                   .IsRequired();

            // OwnerId: required, links Deck to Player
            builder.Property(d => d.OwnerId)
                   .IsRequired();

            // Relationship: Deck belongs to a Player
            builder.HasOne(d => d.Owner)
                   .WithMany(p => p.Decks)
                   .HasForeignKey(d => d.OwnerId)
                   .OnDelete(DeleteBehavior.Cascade); // Delete Deck if Player is deleted

            // Relationship: Deck belongs to an Archetype
            builder.HasOne(d => d.Archetype)
                   .WithMany(a => a.Decks)
                   .HasForeignKey(d => d.ArchetypeId)
                   .OnDelete(DeleteBehavior.Cascade); // Delete Decks if Archetype is deleted

            // CreatedAt: default value set to UTC now
            builder.Property(a => a.CreatedAt)
                   .HasDefaultValueSql("(timezone('utc', now()))")
                   .IsRequired();
        }
    }
}
