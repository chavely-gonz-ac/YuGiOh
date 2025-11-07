using System.Text.Json;
using Microsoft.Extensions.Logging;
using YuGiOh.Domain.Exceptions;

namespace YuGiOh.Infrastructure.CachingService
{
    /// <summary>
    /// Provides grouped cache query functionality for retrieving and filtering collections 
    /// of cached entities with automatic fallback and refresh logic.
    /// </summary>
    public partial class RedisCachingService
    {
        /// <summary>
        /// Retrieves a filtered collection of cached items of type <typeparamref name="T"/>.
        /// If the cache is empty or corrupted, this method loads the data from a source 
        /// using the specified <paramref name="loader"/> function, caches it, and then filters the results.
        /// </summary>
        /// <typeparam name="T">The type of entity being queried.</typeparam>
        /// <param name="predicate">A filter function applied to the cached or loaded data.</param>
        /// <param name="loader">A function that asynchronously loads the full dataset when the cache is empty or invalid.</param>
        /// <param name="ttl">Optional custom time-to-live for the cached dataset.</param>
        /// <param name="cacheKey">
        /// Optional cache key. If not provided, the key will be built automatically using <see cref="GetCachePrefix"/>.
        /// </param>
        /// <returns>
        /// A filtered collection of <typeparamref name="T"/> items that match the given <paramref name="predicate"/>.
        /// </returns>
        /// <exception cref="APIException">
        /// Thrown when a required argument is null or the cache operation fails.
        /// </exception>
        public async Task<ICollection<T>> GroupQuery<T>(
            Func<T, bool> predicate,
            Func<Task<ICollection<T>>> loader,
            TimeSpan? ttl = null,
            string? cacheKey = null)
        {
            if (predicate == null)
                throw APIException.BadRequest("Predicate cannot be null.", nameof(predicate));
            if (loader == null)
                throw APIException.BadRequest("Loader function cannot be null.", nameof(loader));

            cacheKey ??= GetCachePrefix(typeof(T).Name);

            try
            {
                // Attempt to retrieve data from cache
                var cached = await GetAsync<ICollection<T>>(cacheKey).ConfigureAwait(false);
                if (cached is { Count: > 0 })
                {
                    _logger.LogDebug("Cache hit for {Type} ({Count} items)", typeof(T).Name, cached.Count);
                    return cached.Where(predicate).ToList();
                }
            }
            catch (JsonException ex)
            {
                // If the cached entry is corrupted, remove it and continue
                _logger.LogWarning(ex, "Corrupted cache for {Key}, clearing entry.", cacheKey);
                await RemoveAsync<ICollection<T>>(cacheKey).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to query cache for key {Key}", cacheKey);
                throw APIException.Internal(
                    "Error occurred while querying Redis cache.",
                    ex.Message,
                    ex);
            }

            // Cache miss: load data from the provided loader
            ICollection<T> collection;
            try
            {
                collection = await loader().ConfigureAwait(false) ?? new List<T>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Loader function failed for {Type}", typeof(T).Name);
                throw APIException.Internal(
                    "Failed to load data source for caching.",
                    ex.Message,
                    ex);
            }

            // Populate cache if data is available
            if (collection.Count > 0)
            {
                try
                {
                    var expiry = ttl ?? TimeSpan.FromSeconds(_options.DefaultTTLSeconds);
                    await SetAsync(cacheKey, collection, expiry).ConfigureAwait(false);
                    _logger.LogDebug("Cache populated for {Type} ({Count} items)", typeof(T).Name, collection.Count);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to set cache for {Type}", typeof(T).Name);
                    throw APIException.Internal(
                        "Failed to populate Redis cache after loading data.",
                        ex.Message,
                        ex);
                }
            }
            else
            {
                _logger.LogDebug("Loader returned empty result for {Type}", typeof(T).Name);
            }

            return collection.Where(predicate).ToList();
        }
    }
}
