using YuGiOh.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YuGiOh.Domain.Exceptions;

namespace YuGiOh.Infrastructure.EmailService
{
    /// <summary>
    /// Provides extension methods for registering the email service components
    /// (SMTP sender and provider) into the dependency injection container.
    /// </summary>
    public static class ServiceExtension
    {
        /// <summary>
        /// Registers the email sending and templating services into the dependency injection system.
        /// </summary>
        /// <param name="services">The dependency injection service collection.</param>
        /// <param name="configuration">The application configuration source.</param>
        /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
        /// <exception cref="APIException">
        /// Thrown if <paramref name="services"/> or <paramref name="configuration"/> is null.
        /// </exception>
        public static IServiceCollection AddEmailService(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw APIException.BadRequest("Service collection cannot be null.", nameof(services));

            if (configuration == null)
                throw APIException.BadRequest("Configuration instance cannot be null.", nameof(configuration));

            // Binds SMTP options from configuration and ensures they are available
            services.AddOptions<SMTPOptions>()
                    .Bind(configuration.GetSection("SMTPOptions"))
                    .ValidateDataAnnotations()   // optional, triggers [Required] attributes if added
                    .ValidateOnStart();          // fail early if SMTP configuration is invalid

            // Registers the core email infrastructure components
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IEmailProvider, EmailProvider>();

            return services;
        }
    }
}
