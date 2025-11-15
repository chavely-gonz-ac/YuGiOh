using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YuGiOh.Domain.Models;

namespace YuGiOh.Infrastructure.Configurations
{
    /// <summary>
    /// EF Core configuration for the <see cref="Round"/> entity.
    /// </summary>
    public class RoundConfiguration : IEntityTypeConfiguration<Round>
    {
        public void Configure(EntityTypeBuilder<Round> builder)
        {
            // === Table ===
            builder.ToTable("Rounds");

            // === Primary Key ===
            builder.HasKey(r => r.Id);

            // === Enum ===
            builder.Property(r => r.Type)
                .IsRequired()
                .HasConversion<string>()
                .HasComment("Type of round (Classification, KnockOut).");

            // === Tournament Relationship ===
            builder.HasOne(r => r.Tournament)
                .WithMany(t => t.Rounds)
                .HasForeignKey(r => r.TournamentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Rounds_Tournaments");

            builder.Property(r => r.TournamentId)
                .IsRequired();

            // === Matches Relationship ===
            builder.HasMany(r => r.Matches)
                .WithOne(m => m.Round)
                .HasForeignKey(m => m.RoundId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Matches_Rounds");

            // === CreatedAt ===
            builder.Property(r => r.CreatedAt)
                   .HasDefaultValueSql("(timezone('utc', now()))")
                   .IsRequired()
                   .HasComment("Timestamp indicating when the registration was created.");

            // === Indexing ===
            builder.HasIndex(r => r.TournamentId)
                .HasDatabaseName("IX_Rounds_Tournament");
        }
    }
}
