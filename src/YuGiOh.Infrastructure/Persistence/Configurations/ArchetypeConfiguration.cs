// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;
// using YuGiOh.Domain.Models;

// namespace YuGiOh.Infrastructure.Persistence.Configurations
// {
//     public class ArchetypeConfiguration : IEntityTypeConfiguration<Archetype>
//     {
//         public void Configure(EntityTypeBuilder<Archetype> builder)
//         {
//             builder.ToTable("Archetypes");

//             builder.HasKey(a => a.Id);

//             builder.Property(a => a.Name)
//                 .IsRequired()
//                 .HasMaxLength(100);

//             // Relationships
//             builder.HasMany(a => a.Decks)
//                 .WithOne(d => d.Archetype)
//                 .HasForeignKey(d => d.ArchetypeId)
//                 .OnDelete(DeleteBehavior.Restrict);
//         }
//     }
// }
