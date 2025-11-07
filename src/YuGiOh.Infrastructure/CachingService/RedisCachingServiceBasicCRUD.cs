using System.Text.Json;
using Microsoft.Extensions.Logging;
using YuGiOh.Domain.Exceptions;

namespace YuGiOh.Infrastructure.CachingService
{
    /// <summary>
    /// Provides CRUD-style operations for Redis-based caching, including methods to 
    /// set, retrieve, remove, and check for cached entries.
    /// </summary>
    /// <remarks>
    /// This class represents one part of the <see cref="RedisCachingService"/> partial definition.
    /// It is designed for generic usage and type-safe serialization/deserialization using <see cref="JsonSerializer"/>.
    /// </remarks>
    public partial class RedisCachingService
    {
        /// <summary>
        /// Stores a value in Redis cache with the specified key and optional expiration.
        /// </summary>
        /// <typeparam name="T">The type of the value being cached.</typeparam>
        /// <param name="key">The unique cache key identifying the entry.</param>
        /// <param name="value">The value to be cached.</param>
        /// <param name="expiry">
        /// Optional expiration time. If not provided, the default TTL from <see cref="RedisCacheOptions.DefaultTTLSeconds"/> is used.
        /// </param>
        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw APIException.BadRequest("Cache key cannot be null or whitespace.", nameof(key));

            try
            {
                // Serialize the value to JSON and set it in Redis with TTL
                var serialized = JsonSerializer.Serialize(value, _jsonOptions);
                var ttl = expiry ?? TimeSpan.FromSeconds(_options.DefaultTTLSeconds);
                await _db.StringSetAsync(GetCacheKey(typeof(T).Name, key), serialized, ttl)
                         .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cache key {Key} for type {Type}", key, typeof(T).Name);
                throw APIException.Internal("Failed to write to Redis cache.", ex.Message, ex);
            }
        }

        /// <summary>
        /// Retrieves a cached value by key and deserializes it to the specified type.
        /// </summary>
        /// <typeparam name="T">The expected type of the cached value.</typeparam>
        /// <param name="key">The cache key to look up.</param>
        /// <returns>
        /// The cached object if found and successfully deserialized; otherwise, <c>default(T)</c>.
        /// </returns>
        public async Task<T?> GetAsync<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw APIException.BadRequest("Cache key cannot be null or whitespace.", nameof(key));

            var cacheKey = GetCacheKey(typeof(T).Name, key);
            try
            {
                var val = await _db.StringGetAsync(cacheKey).ConfigureAwait(false);
                if (!val.HasValue) return default;

                // Deserialize JSON value to the target type
                return JsonSerializer.Deserialize<T>(val!, _jsonOptions);
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Corrupted cache for key {Key}, removing entry.", cacheKey);
                await _db.KeyDeleteAsync(cacheKey);
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading cache key {Key}", cacheKey);
                throw APIException.Internal("Failed to read from Redis cache.", ex.Message, ex);
            }
        }

        /// <summary>
        /// Attempts to retrieve a cached value; if not found, executes a factory function 
        /// to create it, stores it in cache, and returns the result.
        /// </summary>
        /// <typeparam name="T">The type of the cached value.</typeparam>
        /// <param name="key">The cache key used to identify the entry.</param>
        /// <param name="factory">A function that generates the value when it is not found in cache.</param>
        /// <param name="expiry">Optional expiration time for the cached value.</param>
        /// <returns>
        /// The cached or newly created value.
        /// </returns>
        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw APIException.BadRequest("Cache key cannot be null or whitespace.", nameof(key));
            if (factory == null)
                throw APIException.BadRequest("Value factory cannot be null.");

            var cacheKey = GetCacheKey(typeof(T).Name, key);

            // Try retrieving from cache
            var redisVal = await _db.StringGetAsync(cacheKey).ConfigureAwait(false);
            if (redisVal.HasValue)
            {
                try
                {
                    return JsonSerializer.Deserialize<T>(redisVal!, _jsonOptions)!;
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning(ex,
                        "Failed to deserialize cached entry for key {Key}. Removing corrupted data.",
                        cacheKey);
                    await _db.KeyDeleteAsync(cacheKey);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Unexpected error when deserializing cached entry for key {Key}. Removing entry.",
                        cacheKey);
                    await _db.KeyDeleteAsync(cacheKey);
                    throw APIException.Internal("Unexpected error deserializing Redis cache entry.", ex.Message, ex);
                }
            }

            // Cache miss: call factory to get value
            try
            {
                var result = await factory().ConfigureAwait(false);
                if (result != null)
                    await SetAsync(key, result, expiry);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating or storing value for cache key {Key}", key);
                throw APIException.Internal("Failed to generate or cache a value.", ex.Message, ex);
            }
        }

        /// <summary>
        /// Removes a cached entry for the given key and type.
        /// </summary>
        /// <typeparam name="T">The type of the cached value.</typeparam>
        /// <param name="key">The cache key to remove.</param>
        public async Task RemoveAsync<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw APIException.BadRequest("Cache key cannot be null or whitespace.", nameof(key));

            try
            {
                await _db.KeyDeleteAsync(GetCacheKey(typeof(T).Name, key)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove cache key {Key}", key);
                throw APIException.Internal("Failed to remove cache entry from Redis.", ex.Message, ex);
            }
        }

        /// <summary>
        /// Checks whether a cached entry exists for the given key and type.
        /// </summary>
        /// <typeparam name="T">The type of the cached value.</typeparam>
        /// <param name="key">The cache key to check.</param>
        /// <returns><c>true</c> if the cache entry exists; otherwise, <c>false</c>.</returns>
        public async Task<bool> ExistsAsync<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw APIException.BadRequest("Cache key cannot be null or whitespace.", nameof(key));

            try
            {
                return await _db.KeyExistsAsync(GetCacheKey(typeof(T).Name, key)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check existence of cache key {Key}", key);
                throw APIException.Internal("Failed to check Redis cache entry existence.", ex.Message, ex);
            }
        }
    }
}
