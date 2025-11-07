using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YuGiOh.Domain.Models;

namespace YuGiOh.Infrastructure.Persistence.Configurations
{
       /// <summary>
       /// Configures the persistence mapping for the <see cref="RefreshTokenData"/> entity.
       /// Defines how refresh tokens are stored, constrained, and indexed within the database.
       /// </summary>
       public class RefreshTokenDataConfiguration : IEntityTypeConfiguration<RefreshTokenData>
       {
              /// <summary>
              /// Applies the configuration for the <see cref="RefreshTokenData"/> entity.
              /// </summary>
              /// <param name="builder">The entity builder used for configuration.</param>
              public void Configure(EntityTypeBuilder<RefreshTokenData> builder)
              {
                     // Map to "RefreshTokens" table.
                     builder.ToTable("RefreshTokens");

                     // Set the primary key as the token string itself.
                     builder.HasKey(r => r.Token);

                     // Configure token column:
                     // - Required
                     // - Max length of 200 (supports JWT or UUID-like tokens)
                     builder.Property(r => r.Token)
                            .IsRequired()
                            .HasMaxLength(200);

                     // AccountId: reference to the Account entity (string-based Id from Identity)
                     builder.Property(r => r.AccountId)
                            .IsRequired()
                            .HasMaxLength(50);

                     // CreatedByIp: IP address where the token was generated
                     builder.Property(r => r.CreatedByIp)
                            .IsRequired()
                            .HasMaxLength(45); // IPv6-safe

                     // Optional RevokedByIp: the IP address that revoked the token
                     builder.Property(r => r.RevokedByIp)
                            .HasMaxLength(45);

                     // Optional ReplacedByToken: link to new token when rotated
                     builder.Property(r => r.ReplacedByToken)
                            .HasMaxLength(200);

                     // Dates (timestamps)
                     builder.Property(r => r.Created)
                            .IsRequired()
                            .HasDefaultValueSql("(timezone('utc', now()))");

                     builder.Property(r => r.Expires)
                            .IsRequired();

                     builder.Property(r => r.Revoked)
                            .IsRequired(false);

                     // Add an index on AccountId for faster lookups
                     builder.HasIndex(r => r.AccountId);

                     // Enforce that a user cannot have duplicate active tokens
                     // via a unique filtered index (PostgreSQL only):
                     builder.HasIndex(r => new { r.AccountId, r.Revoked })
                            .HasFilter("\"Revoked\" IS NULL")
                            .IsUnique();
              }
       }
}
