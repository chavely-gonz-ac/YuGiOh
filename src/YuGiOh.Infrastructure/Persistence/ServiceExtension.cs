using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YuGiOh.Infrastructure.Persistence.Repositories;

namespace YuGiOh.Infrastructure.Persistence
{
    /// <summary>
    /// Provides extension methods for registering the persistence layer services,
    /// including the Entity Framework Core DbContext and repositories.
    /// </summary>
    public static class ServiceExtension
    {
        /// <summary>
        /// Registers the <see cref="YuGiOhDbContext"/> with PostgreSQL provider
        /// and the generic repositories for data access.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <param name="configuration">The application configuration (used for connection string).</param>
        /// <returns>The same <see cref="IServiceCollection"/> instance for chaining.</returns>
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure DbContext with PostgreSQL provider
            services.AddDbContext<YuGiOhDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    npgsqlOptions =>
                    {
                        npgsqlOptions.MigrationsAssembly(typeof(YuGiOhDbContext).Assembly.FullName);
                        npgsqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(5),
                            errorCodesToAdd: null);
                    })
            // Optional: optimize for read-heavy workloads
            // .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            );

            // Register generic repository implementations using Ardalis.Specification
            services.AddScoped(typeof(IRepositoryBase<>), typeof(DataRepository<>));
            services.AddScoped(typeof(IReadRepositoryBase<>), typeof(DataRepository<>));

            return services;
        }
    }
}
