// ===== CachingService/ServiceExtension.cs =====
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using YuGiOh.Domain.Repositories;
using YuGiOh.Domain.Exceptions;

namespace YuGiOh.Infrastructure.CachingService
{
    /// <summary>
    /// Provides extension methods for registering Redis-based caching services
    /// into the dependency injection container.
    /// </summary>
    public static class ServiceExtension
    {
        /// <summary>
        /// Adds the Redis caching infrastructure and its dependencies to the application's service collection.
        /// </summary>
        /// <param name="services">The dependency injection service collection.</param>
        /// <param name="configuration">The application's configuration instance.</param>
        /// <returns>The same <see cref="IServiceCollection"/> instance for fluent chaining.</returns>
        /// <exception cref="APIException">
        /// Thrown when Redis configuration is missing, invalid, or connection initialization fails.
        /// </exception>
        public static IServiceCollection AddCachingService(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw APIException.BadRequest("Service collection cannot be null.", nameof(services));

            if (configuration == null)
                throw APIException.BadRequest("Configuration instance cannot be null.", nameof(configuration));

            // Bind and validate Redis configuration options
            services.Configure<RedisCacheOptions>(configuration.GetSection("RedisCacheOptions"));

            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                try
                {
                    var opts = sp.GetRequiredService<IOptions<RedisCacheOptions>>().Value;

                    if (string.IsNullOrWhiteSpace(opts.Configuration))
                        throw APIException.BadRequest("Redis configuration string cannot be null or empty.");

                    // Parse Redis connection configuration
                    var cfg = ConfigurationOptions.Parse(opts.Configuration);
                    cfg.AbortOnConnectFail = false; // Prevents app from crashing if Redis temporarily unavailable

                    var connection = ConnectionMultiplexer.Connect(cfg);

                    // Validate connectivity immediately to fail fast on startup
                    if (!connection.IsConnected)
                        throw APIException.Internal("Redis connection failed during initialization.");

                    return connection;
                }
                catch (APIException)
                {
                    // Rethrow known structured exceptions directly
                    throw;
                }
                catch (RedisConnectionException ex)
                {
                    throw APIException.Internal(
                        "Unable to establish connection with Redis server.",
                        ex.Message,
                        ex);
                }
                catch (Exception ex)
                {
                    throw APIException.Internal(
                        "An unexpected error occurred while configuring Redis connection.",
                        ex.Message,
                        ex);
                }
            });

            // Register Redis-based caching service as the implementation for the caching repository abstraction
            services.AddSingleton<ICachingRepository, RedisCachingService>();

            return services;
        }
    }
}
