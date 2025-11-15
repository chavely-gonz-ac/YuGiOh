using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YuGiOh.Domain.Models;

namespace YuGiOh.Infrastructure.Configurations
{
    /// <summary>
    /// EF Core configuration for the <see cref="Match"/> entity.
    /// </summary>
    public class MatchConfiguration : IEntityTypeConfiguration<Match>
    {
        public void Configure(EntityTypeBuilder<Match> builder)
        {
            // === Table ===
            builder.ToTable("Matches");

            // === Primary Key ===
            builder.HasKey(m => m.Id);

            // === Round Relationship ===
            builder.Property(m => m.RoundId)
                .IsRequired()
                .HasComment("FK to the Round this match belongs to.");

            builder.HasOne(m => m.Round)
                .WithMany(r => r.Matches)
                .HasForeignKey(m => m.RoundId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Matches_Rounds");

            // === Match State ===
            builder.Property(m => m.IsRunning)
                .IsRequired()
                .HasDefaultValue(false)
                .HasComment("Indicates whether the match is currently in progress.");

            builder.Property(m => m.IsFinished)
                .IsRequired()
                .HasDefaultValue(false)
                .HasComment("Indicates whether the match has been completed.");

            builder.Property(m => m.StartDate)
                .IsRequired()
                .HasComment("Exact timestamp when the match started.");

            // === White Player Relationship ===
            builder.Property(m => m.WhitePlayerId)
                .IsRequired()
                .HasComment("User ID of the white-side player.");

            builder.HasOne(m => m.WhitePlayer)
                .WithMany() // If Player has MatchesAsWhite, replace with .WithMany(p => p.MatchesAsWhite)
                .HasForeignKey(m => m.WhitePlayerId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Matches_Players_White");

            builder.Property(m => m.WhitePlayerResult)
                .IsRequired()
                .HasDefaultValue(0)
                .HasComment("Result score of the white player (0-2).");

            // === Black Player Relationship ===
            builder.Property(m => m.BlackPlayerId)
                .IsRequired()
                .HasComment("User ID of the black-side player.");

            builder.HasOne(m => m.BlackPlayer)
                .WithMany() // If Player has MatchesAsBlack, replace with .WithMany(p => p.MatchesAsBlack)
                .HasForeignKey(m => m.BlackPlayerId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Matches_Players_Black");

            builder.Property(m => m.BlackPlayerResult)
                .IsRequired()
                .HasDefaultValue(0)
                .HasComment("Result score of the black player (0-2).");

            // === CreatedAt ===
            builder.Property(m => m.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Timestamp indicating when this match was created.");

            // === Indexes ===

            // Prevent the same two players from playing twice in the same round
            builder.HasIndex(m => new { m.RoundId, m.WhitePlayerId, m.BlackPlayerId })
                .IsUnique()
                .HasDatabaseName("IX_Matches_Round_White_Black");
        }
    }
}
