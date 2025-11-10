using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YuGiOh.Domain.Models;

namespace YuGiOh.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Configures the <see cref="Sponsor"/> entity for EF Core.
    /// </summary>
    public class SponsorConfiguration : IEntityTypeConfiguration<Sponsor>
    {
        public void Configure(EntityTypeBuilder<Sponsor> builder)
        {
            builder.ToTable("Sponsors");

            builder.HasKey(x => x.Id);

            builder.HasMany(s => s.Sponsorships)
                .WithOne(sp => sp.Sponsor)
                .HasForeignKey(sp => sp.SponsorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
