using YuGiOh.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YuGiOh.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Configures the Entity Framework Core persistence mapping for the <see cref="StreetType"/> entity.
    /// This configuration ensures proper indexing, constraints, and table naming conventions
    /// for the street type data stored in the database.
    /// </summary>
    public class StreetTypeConfiguration : IEntityTypeConfiguration<StreetType>
    {
        /// <summary>
        /// Applies the configuration for the <see cref="StreetType"/> entity.
        /// </summary>
        /// <param name="builder">The entity type builder used to configure this entity.</param>
        public void Configure(EntityTypeBuilder<StreetType> builder)
        {
            // Map entity to "StreetTypes" table.
            // Table naming convention: pluralized, PascalCase.
            builder.ToTable("StreetTypes");

            // Define the primary key.
            builder.HasKey(s => s.Id);

            // Configure "Name" column:
            //  - Required
            //  - Maximum length of 50 characters
            //  - Indexed uniquely (no duplicates)
            builder.Property(s => s.Name)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.HasIndex(s => s.Name)
                   .IsUnique();
        }
    }
}
