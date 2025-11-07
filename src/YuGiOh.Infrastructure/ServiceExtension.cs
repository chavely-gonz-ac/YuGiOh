using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YuGiOh.Infrastructure.CachingService;
using YuGiOh.Infrastructure.CSCService;
using YuGiOh.Infrastructure.EmailService;
using YuGiOh.Infrastructure.Identity;
using YuGiOh.Infrastructure.Persistence;
using YuGiOh.Infrastructure.Seeding;

namespace YuGiOh.Infrastructure
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddInfrastructureLayer
        (
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddEmailService(configuration);
            services.AddPersistence(configuration);
            services.AddCachingService(configuration);
            services.AddCSCService(configuration);
            services.AddIdentityService(configuration);
            services.AddSeeding();
            return services;
        }
    }
}
