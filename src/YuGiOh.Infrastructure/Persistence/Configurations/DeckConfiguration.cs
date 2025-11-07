// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;
// using YuGiOh.Domain.Models;

// namespace YuGiOh.Infrastructure.Persistence.Configurations
// {
//     public class DeckConfiguration : IEntityTypeConfiguration<Deck>
//     {
//         public void Configure(EntityTypeBuilder<Deck> builder)
//         {
//             builder.ToTable("Decks");

//             builder.HasKey(d => d.Id);

//             builder.Property(d => d.Name)
//                 .IsRequired()
//                 .HasMaxLength(100);

//             builder.Property(d => d.MainDeckSize)
//                 .IsRequired();

//             builder.Property(d => d.SideDeckSize)
//                 .IsRequired();

//             builder.Property(d => d.ExtraDeckSize)
//                 .IsRequired();

//             builder.Property(d => d.CreatedAt)
//                 .IsRequired()
//                 .HasDefaultValueSql("CURRENT_TIMESTAMP");

//             // ✅ Range constraints
//             builder.HasCheckConstraint("CK_Deck_MainDeckSize_Range", "[MainDeckSize] BETWEEN 40 AND 60");
//             builder.HasCheckConstraint("CK_Deck_SideDeckSize_Range", "[SideDeckSize] BETWEEN 0 AND 15");
//             builder.HasCheckConstraint("CK_Deck_ExtraDeckSize_Range", "[ExtraDeckSize] BETWEEN 0 AND 15");

//             // Relationships
//             builder.HasOne(d => d.Archetype)
//                 .WithMany(a => a.Decks)
//                 .HasForeignKey(d => d.ArchetypeId)
//                 .OnDelete(DeleteBehavior.Restrict);

//             builder.HasOne(d => d.Owner)
//                 .WithMany(p => p.Decks)
//                 .HasForeignKey(d => d.OwnerId)
//                 .OnDelete(DeleteBehavior.Cascade);
//         }
//     }
// }
