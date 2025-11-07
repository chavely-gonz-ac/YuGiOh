using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

using YuGiOh.Domain.Services;
using YuGiOh.Domain.Exceptions;
using YuGiOh.Infrastructure.Identity.Services;
using YuGiOh.Infrastructure.Persistence;

namespace YuGiOh.Infrastructure.Identity
{
    /// <summary>
    /// Provides extension methods for configuring identity, authentication, and authorization services
    /// within the dependency injection container.
    /// </summary>
    public static class ServiceExtension
    {
        /// <summary>
        /// Registers and configures ASP.NET Core Identity, JWT authentication,
        /// and related identity services for the Yu-Gi-Oh! application.
        /// </summary>
        /// <param name="services">The dependency injection service collection.</param>
        /// <param name="configuration">The application configuration source.</param>
        /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
        /// <exception cref="APIException">
        /// Thrown when the service collection, configuration, or JWT settings are invalid.
        /// </exception>
        public static IServiceCollection AddIdentityService(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw APIException.BadRequest("Service collection cannot be null.", nameof(services));

            if (configuration == null)
                throw APIException.BadRequest("Configuration instance cannot be null.", nameof(configuration));

            // === Identity Core Setup ===
            services
                .AddIdentity<Account, IdentityRole>(options =>
                {
                    // User settings
                    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                    options.User.RequireUniqueEmail = true;

                    // Sign-in settings
                    options.SignIn.RequireConfirmedEmail = true;

                    // Password policy
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequiredLength = 8;

                    // Lockout policy
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                    options.Lockout.AllowedForNewUsers = true;
                })
                .AddEntityFrameworkStores<YuGiOhDbContext>()
                .AddDefaultTokenProviders();

            // === JWT Authentication Setup ===
            services.Configure<JWTOptions>(configuration.GetSection("JWTOptions"));
            var jwtOptions = configuration.GetSection("JWTOptions").Get<JWTOptions>();

            if (jwtOptions == null)
                throw APIException.BadRequest("JWTOptions section is missing or misconfigured.");

            if (string.IsNullOrWhiteSpace(jwtOptions.SecretKey))
                throw APIException.BadRequest("JWT SecretKey cannot be null or empty.");

            if (jwtOptions.SecretKey.Length < 32)
                throw APIException.BadRequest("JWT SecretKey must be at least 32 characters long for sufficient cryptographic strength.");

            if (string.IsNullOrWhiteSpace(jwtOptions.Issuer))
                throw APIException.BadRequest("JWT Issuer cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(jwtOptions.Audience))
                throw APIException.BadRequest("JWT Audience cannot be null or empty.");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = true; // enforce HTTPS
                o.SaveToken = true;

                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1),
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                };

                // Custom JWT failure responses
                o.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = context =>
                    {
                        context.NoResult();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(new
                        {
                            error = "Authentication failed",
                            details = context.Exception.Message
                        });
                        return context.Response.WriteAsync(result);
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(new
                        {
                            error = "Unauthorized",
                            message = "You must be logged in to access this resource."
                        });
                        return context.Response.WriteAsync(result);
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(new
                        {
                            error = "Forbidden",
                            message = "You do not have access to this resource."
                        });
                        return context.Response.WriteAsync(result);
                    }
                };
            });

            // === Custom Identity Services ===
            services.AddScoped<RolesSpecificEntitiesManager>();
            services.AddScoped<IAccountTokensProvider, AccountTokensProvider>();
            services.AddScoped<IRegisterHandler, RegisterHandler>();
            services.AddScoped<IAuthenticationHandler, AuthenticationHandler>();

            return services;
        }
    }
}
