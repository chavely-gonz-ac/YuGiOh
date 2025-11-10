using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YuGiOh.Domain.Models;

namespace YuGiOh.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Configures the <see cref="Sponsorship"/> entity for EF Core.
    /// </summary>
    public class SponsorshipConfiguration : IEntityTypeConfiguration<Sponsorship>
    {
        public void Configure(EntityTypeBuilder<Sponsorship> builder)
        {
            builder.ToTable("Sponsorships");

            builder.HasKey(s => new { s.SponsorId, s.TournamentId });

            builder.Property(s => s.CreatedAt)
                .HasDefaultValueSql("(timezone('utc', now()))")
                .IsRequired();

            builder.HasOne(s => s.Sponsor)
                .WithMany(sp => sp.Sponsorships)
                .HasForeignKey(s => s.SponsorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.Tournament)
                .WithMany(t => t.SponsoredBy)
                .HasForeignKey(s => s.TournamentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
