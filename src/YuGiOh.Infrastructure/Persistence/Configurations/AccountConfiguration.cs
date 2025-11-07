using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YuGiOh.Infrastructure.Identity;

namespace YuGiOh.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Configures the Entity Framework Core mapping for the <see cref="Account"/> entity.
    /// This configuration defines how Identity accounts are persisted in the database,
    /// ensuring proper defaults, constraints, and index structures.
    /// </summary>
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        /// <summary>
        /// Configures the <see cref="Account"/> entity properties and constraints.
        /// </summary>
        /// <param name="builder">The EntityTypeBuilder used to configure the entity.</param>
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.Property(a => a.CreatedAt)
                   .HasDefaultValueSql("(timezone('utc', now()))")
                   .IsRequired();

            // Ensure that the email and username are unique in the database for Identity accounts
            builder.HasIndex(a => a.Email)
                   .IsUnique();

            builder.HasIndex(a => a.UserName)
                   .IsUnique();


        }
    }
}
