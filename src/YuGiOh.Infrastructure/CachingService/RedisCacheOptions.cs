using System.ComponentModel.DataAnnotations;

namespace YuGiOh.Infrastructure.CachingService
{
    /// <summary>
    /// Represents the configuration settings used for connecting to a Redis cache instance.
    /// These options are typically bound from the application's configuration file (e.g., appsettings.json)
    /// under a section such as <c>"RedisCacheOptions"</c>.
    /// </summary>
    public class RedisCacheOptions
    {
        /// <summary>
        /// Gets or sets the connection string used to establish a connection to the Redis server.
        /// This property is required and must contain a valid configuration string 
        /// (e.g., "localhost:6379" or "myredis:6380,password=secret,ssl=True").
        /// </summary>
        [Required]
        public required string Configuration { get; set; }

        /// <summary>
        /// Gets or sets an optional name prefix for this Redis instance.
        /// This can be used to isolate cache keys between different environments 
        /// (e.g., "Dev", "Prod") or subsystems within the same Redis server.
        /// </summary>
        public string? InstanceName { get; set; }

        /// <summary>
        /// Gets or sets the default time-to-live (TTL) in seconds for cache entries.
        /// If not specified when setting a value in cache, this value determines 
        /// how long an entry will remain valid before expiring automatically.
        /// The default is 600 seconds (10 minutes).
        /// </summary>
        public int DefaultTTLSeconds { get; set; } = 600;

        /// <summary>
        /// Gets or sets the separator string used to construct composite cache keys.
        /// The default separator is a colon (<c>":"</c>), resulting in keys like <c>"InstanceName:Type:Id"</c>.
        /// </summary>
        public string KeySeparator { get; set; } = ":";
    }
}
