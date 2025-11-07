using YuGiOh.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using YuGiOh.Domain.Exceptions;

namespace YuGiOh.Infrastructure.CSCService
{
    /// <summary>
    /// Provides service registration extensions for integrating the CSC (Country-State-City)
    /// API into the dependency injection container.
    /// </summary>
    public static class ServiceExtension
    {
        /// <summary>
        /// Registers the CSC API client and related provider services within the dependency injection system.
        /// </summary>
        /// <param name="services">The dependency injection service collection.</param>
        /// <param name="configuration">The application configuration instance.</param>
        /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
        /// <exception cref="APIException">
        /// Thrown if the configuration or required CSC options are missing or invalid.
        /// </exception>
        public static IServiceCollection AddCSCService(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw APIException.BadRequest("Service collection cannot be null.", nameof(services));

            if (configuration == null)
                throw APIException.BadRequest("Configuration instance cannot be null.", nameof(configuration));

            // Bind and validate CSC configuration options
            services.Configure<CSCOptions>(configuration.GetSection("CSCOptions"));

            // Configure the HttpClient for CSC API with automatic key and base URL setup
            services.AddHttpClient<CSCLoader>((sp, client) =>
            {
                try
                {
                    var opts = sp.GetRequiredService<IOptions<CSCOptions>>().Value;

                    if (string.IsNullOrWhiteSpace(opts.Endpoint))
                        throw APIException.BadRequest("CSC API endpoint cannot be null or empty.");

                    if (string.IsNullOrWhiteSpace(opts.APIKey))
                        throw APIException.BadRequest("CSC API key cannot be null or empty.");

                    client.BaseAddress = new Uri(opts.Endpoint.TrimEnd('/') + "/");

                    if (!client.DefaultRequestHeaders.TryAddWithoutValidation("X-CSCAPI-KEY", opts.APIKey))
                        throw APIException.Internal("Failed to add CSC API key to request headers.");
                }
                catch (APIException)
                {
                    // Forward structured exceptions unchanged
                    throw;
                }
                catch (Exception ex)
                {
                    throw APIException.Internal(
                        "Unexpected error occurred while configuring CSC HttpClient.",
                        ex.Message,
                        ex);
                }
            });

            // Register the CSCProvider as a scoped service
            services.AddScoped<ICSCProvider, CSCProvider>();

            return services;
        }
    }
}
