using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YuGiOh.Domain.Models;

namespace YuGiOh.Infrastructure.Configurations
{
    /// <summary>
    /// Configures the database schema for the <see cref="Registration"/> entity.
    /// </summary>
    /// <remarks>
    /// A Registration represents the participation of a Deck in a specific Tournament.
    /// This configuration defines the table name, primary key, relationships, constraints,
    /// property mappings, and indexing strategy.
    /// </remarks>
    public class RegistrationConfiguration : IEntityTypeConfiguration<Registration>
    {
        public void Configure(EntityTypeBuilder<Registration> builder)
        {
            // === Table Mapping ===
            builder.ToTable("Registrations");

            // === Primary Key ===
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id)
                   .ValueGeneratedOnAdd();

            // === Boolean Flags ===
            builder.Property(r => r.Accepted)
                   .IsRequired()
                   .HasDefaultValue(true)
                   .HasComment("Indicates whether the tournament administrator accepted this registration.");

            builder.Property(r => r.IsWinner)
                   .IsRequired()
                   .HasDefaultValue(false)
                   .HasComment("Indicates whether the player won the tournament using this registration.");

            builder.Property(r => r.IsPlaying)
                   .IsRequired()
                   .HasDefaultValue(true)
                   .HasComment("Indicates whether the player is currently active in the tournament.");

            // === Description ===
            builder.Property(r => r.Description)
                   .HasMaxLength(500)
                   .HasComment("Optional textual note about the registration (e.g., special conditions or remarks).");

            // === CreatedAt ===
            builder.Property(r => r.CreatedAt)
                   .HasDefaultValueSql("(timezone('utc', now()))")
                   .IsRequired()
                   .HasComment("Timestamp indicating when the registration was created.");

            // === Relationships ===
            // Each registration belongs to one Deck.
            builder.HasOne(r => r.Deck)
                   .WithMany(d => d.Registrations)
                   .HasForeignKey(r => r.DeckId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .HasConstraintName("FK_Registrations_Decks");

            // Each registration belongs to one Tournament.
            builder.HasOne(r => r.Tournament)
                   .WithMany(t => t.Registrations)
                   .HasForeignKey(r => r.TournamentId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .HasConstraintName("FK_Registrations_Tournaments");

            // === Indexes ===
            builder.HasIndex(r => new { r.TournamentId, r.DeckId })
                   .IsUnique()
                   .HasDatabaseName("IX_Registrations_Tournament_Deck");

            builder.HasIndex(r => r.CreatedAt)
                   .HasDatabaseName("IX_Registrations_CreatedAt");
        }
    }
}
