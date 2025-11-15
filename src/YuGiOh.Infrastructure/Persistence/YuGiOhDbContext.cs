using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using YuGiOh.Domain.Models;
using YuGiOh.Infrastructure.Identity;

namespace YuGiOh.Infrastructure.Persistence
{
    /// <summary>
    /// Represents the Entity Framework Core database context for the Yu-Gi-Oh! application.
    /// Integrates ASP.NET Identity with domain entities such as Players, Addresses, and StreetTypes.
    /// </summary>
    public class YuGiOhDbContext : IdentityDbContext<Account, IdentityRole, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="YuGiOhDbContext"/> class.
        /// </summary>
        /// <param name="options">Database context configuration options.</param>
        public YuGiOhDbContext(DbContextOptions<YuGiOhDbContext> options)
            : base(options)
        {
        }

        // === Domain Entities ===
        public DbSet<Player> Players { get; set; } = default!;
        public DbSet<StreetType> StreetTypes { get; set; } = default!;
        public DbSet<Address> Addresses { get; set; } = default!;
        public DbSet<HasIBAN> HasIBANs { get; set; } = default!;
        public DbSet<RefreshTokenData> RefreshTokensData { get; set; } = default!;
        public DbSet<Sponsor> Sponsors { get; set; } = default!;
        public DbSet<Deck> Decks { get; set; } = default!;
        public DbSet<Tournament> Tournaments { get; set; } = default!;
        public DbSet<Sponsorship> Sponsorships { get; set; } = default!;
        public DbSet<Registration> Registrations { get; set; } = default!;
        public DbSet<Round> Rounds { get; set; } = default!;
        public DbSet<Match> Matches { get; set; } = default!;

        /// <summary>
        /// Configures the entity mappings and Identity schema for the context.
        /// </summary>
        /// <param name="builder">The model builder used to configure entity relationships and mappings.</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Apply ASP.NET Identity configuration
            base.OnModelCreating(builder);

            // Apply all configurations automatically from this assembly
            builder.ApplyConfigurationsFromAssembly(typeof(YuGiOhDbContext).Assembly);

            // Optional: Add schema comment for PostgreSQL
            builder.HasAnnotation("Comment", "Yu-Gi-Oh! Application Persistence Layer Context");
        }

        /// <summary>
        /// Ensures all DateTime fields are stored in UTC before saving to the database.
        /// </summary>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries()
                                               .Where(e => e.Entity is RefreshTokenData &&
                                                           e.State is EntityState.Added or EntityState.Modified))
            {
                if (entry.Property("Created").CurrentValue is DateTime created)
                    entry.Property("Created").CurrentValue = DateTime.SpecifyKind(created, DateTimeKind.Utc);
                if (entry.Property("Expires").CurrentValue is DateTime expires)
                    entry.Property("Expires").CurrentValue = DateTime.SpecifyKind(expires, DateTimeKind.Utc);
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
