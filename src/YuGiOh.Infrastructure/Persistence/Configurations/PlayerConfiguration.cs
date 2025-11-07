using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YuGiOh.Domain.Models;

namespace YuGiOh.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Configures the persistence mapping for the <see cref="Player"/> entity.
    /// Each player represents a registered user who participates in tournaments or matches.
    /// </summary>
    public class PlayerConfiguration : IEntityTypeConfiguration<Player>
    {
        /// <summary>
        /// Applies Entity Framework Core configuration for the <see cref="Player"/> entity.
        /// </summary>
        /// <param name="builder">The entity type builder used for configuration.</param>
        public void Configure(EntityTypeBuilder<Player> builder)
        {
            // Map to "Players" table.
            builder.ToTable("Players");

            // Primary key: string-based ID (matches Account.Id in Identity)
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                   .IsRequired()
                   .HasMaxLength(50);

            // Relationship: Player → Address (many players can share the same address)
            builder.HasOne(p => p.Address)
                   .WithMany(a => a.Players)
                   .HasForeignKey(p => p.AddressId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
