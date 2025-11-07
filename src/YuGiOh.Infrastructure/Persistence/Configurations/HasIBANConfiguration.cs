using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YuGiOh.Domain.Models;
using YuGiOh.Infrastructure.Identity;

namespace YuGiOh.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Configures the persistence mapping for the <see cref="HasIBAN"/> entity.
    /// This table represents any account or user entity that possesses an IBAN,
    /// such as staff, sponsors, or administrative financial accounts.
    /// </summary>
    public class HasIBANConfiguration : IEntityTypeConfiguration<HasIBAN>
    {
        /// <summary>
        /// Applies the configuration for the <see cref="HasIBAN"/> entity.
        /// </summary>
        /// <param name="builder">The entity type builder used for configuration.</param>
        public void Configure(EntityTypeBuilder<HasIBAN> builder)
        {
            // Map to "HasIBAN" table (singular form consistent with your other configs)
            builder.ToTable("HasIBAN");

            // Define primary key (string-based, same as Account.Id)
            builder.HasKey(s => s.Id);

            // Configure the IBAN property:
            //  - Required (every record must have an IBAN)
            //  - Max length of 50 characters (covers all valid IBAN formats)
            builder.Property(s => s.IBAN)
                   .IsRequired()
                   .HasMaxLength(50);

            // Enforce uniqueness: each IBAN can only belong to one entity.
            builder.HasIndex(s => s.IBAN)
                   .IsUnique();

            // Define a relationship to Account if you want automatic cleanup
            builder.HasOne<Account>()
                   .WithOne()
                   .HasForeignKey<HasIBAN>(s => s.Id)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
