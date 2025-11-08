using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YuGiOh.Domain.Models;

namespace YuGiOh.Infrastructure.Persistence.Configurations
{
    public class ArchetypeConfiguration : IEntityTypeConfiguration<Archetype>
    {
        public void Configure(EntityTypeBuilder<Archetype> builder)
        {
            // Table name
            builder.ToTable("Archetypes");

            // Primary key
            builder.HasKey(a => a.Id);

            // Name: required, max length 100
            builder.Property(a => a.Name)
                   .IsRequired()
                   .HasMaxLength(100);
            builder.HasIndex(a => a.Name)
                .IsUnique();

            // Decks: one-to-many relationship
            builder.HasMany(a => a.Decks)
                   .WithOne(d => d.Archetype)
                   .HasForeignKey(d => d.ArchetypeId)
                   .OnDelete(DeleteBehavior.Cascade); // Delete decks if archetype is deleted
        }
    }
}
