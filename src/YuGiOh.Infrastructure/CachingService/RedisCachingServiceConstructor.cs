using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using YuGiOh.Domain.Repositories;
using Microsoft.Extensions.Logging;
using YuGiOh.Domain.Exceptions;

namespace YuGiOh.Infrastructure.CachingService
{
    /// <summary>
    /// Provides Redis-based caching functionality that supports storing, retrieving,
    /// and removing cached data in a strongly typed and consistent way.
    /// </summary>
    /// <remarks>
    /// This is the constructor and setup portion of the <see cref="RedisCachingService"/> class.
    /// It establishes the Redis connection, configures serialization behavior, and 
    /// defines key naming conventions and prefixing.
    /// </remarks>
    public partial class RedisCachingService : ICachingRepository
    {
        private readonly IConnectionMultiplexer _multiplexer;
        private readonly IDatabase _db;
        private readonly RedisCacheOptions _options;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly string _prefix;
        private readonly ILogger<RedisCachingService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisCachingService"/> class,
        /// configuring the Redis connection, JSON serialization, and key prefixing strategy.
        /// </summary>
        /// <param name="multiplexer">The Redis connection multiplexer.</param>
        /// <param name="options">The configuration options for Redis cache.</param>
        /// <param name="logger">The logger instance used for diagnostic and error messages.</param>
        /// <exception cref="APIException">
        /// Thrown when dependencies are null or Redis connection fails during initialization.
        /// </exception>
        public RedisCachingService(
            IConnectionMultiplexer multiplexer,
            IOptions<RedisCacheOptions> options,
            ILogger<RedisCachingService> logger)
        {
            _multiplexer = multiplexer ?? throw APIException.BadRequest(
                "Redis connection multiplexer cannot be null.",
                nameof(multiplexer));

            _options = options?.Value ?? throw APIException.BadRequest(
                "Redis configuration options cannot be null.",
                nameof(options));

            _logger = logger ?? throw APIException.BadRequest(
                "Logger instance cannot be null.",
                nameof(logger));

            try
            {
                _db = _multiplexer.GetDatabase();

                // Validate connection immediately to ensure Redis is reachable
                _ = _db.Ping();
                _logger.LogInformation("Redis connection successfully established and validated.");
            }
            catch (RedisConnectionException ex)
            {
                _logger.LogError(ex, "Unable to connect to Redis during initialization.");
                throw APIException.Internal(
                    "Redis connection failed at startup.",
                    ex.Message,
                    ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve or validate Redis database instance.");
                throw APIException.Internal(
                    "Failed to initialize Redis database connection.",
                    ex.Message,
                    ex);
            }

            // Build cache key prefix based on the configured instance name
            _prefix = string.IsNullOrWhiteSpace(_options.InstanceName)
                ? string.Empty
                : $"{_options.InstanceName}{_options.KeySeparator}";

            // Configure JSON serialization settings
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = false
            };
        }

        /// <summary>
        /// Builds and returns the cache prefix (namespace) for a given logical table name.
        /// </summary>
        /// <param name="tableName">The logical group or table for cached data.</param>
        /// <returns>A composed prefix string to scope cache entries.</returns>
        public string GetCachePrefix(string tableName) => $"{_prefix}{tableName}";

        /// <summary>
        /// Generates a full cache key using the prefix, separator, and logical key.
        /// </summary>
        /// <param name="tableName">The logical cache group (e.g., entity name).</param>
        /// <param name="key">The unique key for the cached entry.</param>
        /// <returns>A fully qualified Redis cache key string.</returns>
        public string GetCacheKey(string tableName, string key)
            => $"{GetCachePrefix(tableName)}{_options.KeySeparator}{key}";
    }
}
