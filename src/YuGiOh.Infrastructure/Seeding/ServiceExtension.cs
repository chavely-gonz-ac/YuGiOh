using Microsoft.Extensions.DependencyInjection;
using YuGiOh.Infrastructure.Seeding.Tools;

namespace YuGiOh.Infrastructure.Seeding
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddSeeding(this IServiceCollection services)
        {
            // Register the tools provider (creates seeding logic for each entity)
            services.AddScoped<SeedingToolsProvider>();

            // Register the background hosted service that runs the seeding process
            services.AddHostedService<DbSeeder>();

            return services;
        }
    }
}
