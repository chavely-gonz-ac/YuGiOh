using YuGiOh.Domain.DTOs;
using YuGiOh.Domain.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YuGiOh.Infrastructure.Persistence.Configurations
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("Addresses");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.StreetName)
                   .HasMaxLength(200);

            builder.Property(a => a.Building)
                   .HasMaxLength(100)
                   .IsRequired(false);

            builder.Property(a => a.Apartment)
                   .HasMaxLength(50)
                   .IsRequired(false);

            builder.Property(a => a.CountryIso2)
                   .IsRequired();

            builder.Property(a => a.StateIso2)
                   .IsRequired(false);

            builder.Property(a => a.City)
                   .IsRequired(false);

            builder.HasOne(a => a.StreetType)
                   .WithMany()
                   .HasForeignKey(a => a.StreetTypeId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
