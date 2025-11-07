// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;

// using YuGiOh.Domain.Models;

// namespace YuGiOh.Infrastructure.Persistence.Configurations
// {
//     public class SponsorConfiguration : IEntityTypeConfiguration<Sponsor>
//     {
//         public void Configure(EntityTypeBuilder<Sponsor> builder)
//         {
//             builder.ToTable("Sponsors");

//             builder.HasKey(s => s.Id);

//             builder.Property(s => s.Id)
//                    .IsRequired()
//                    .HasMaxLength(50);

//             builder.Property(s => s.IBAN)
//                    .HasMaxLength(34)
//                    .IsRequired();

//             // builder.HasMany(s => s.Sponsored)
//             //        .WithOne(sp => sp.Sponsor)
//             //        .HasForeignKey(sp => sp.SponsorId)
//             //        .OnDelete(DeleteBehavior.Restrict);
//         }
//     }
// }
