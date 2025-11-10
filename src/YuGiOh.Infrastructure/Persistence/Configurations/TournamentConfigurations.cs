using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YuGiOh.Domain.Models;

namespace YuGiOh.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Configures the <see cref="Tournament"/> entity for EF Core.
    /// </summary>
    public class TournamentConfiguration : IEntityTypeConfiguration<Tournament>
    {
        public void Configure(EntityTypeBuilder<Tournament> builder)
        {
            builder.ToTable("Tournaments");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.StartDate)
                .IsRequired();

            builder.Property(t => t.RegistrationLimit)
                .IsRequired();

            builder.Property(t => t.CreatedAt)
                .IsRequired();

            builder.HasOne(t => t.Address)
                .WithMany(a => a.Tournaments) // Address can be reused if needed; adjust if not desired
                .HasForeignKey(t => t.AddressId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(t => t.SponsoredBy)
                .WithOne(s => s.Tournament)
                .HasForeignKey(s => s.TournamentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
