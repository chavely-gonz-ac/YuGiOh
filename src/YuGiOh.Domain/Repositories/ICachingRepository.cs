namespace YuGiOh.Domain.Repositories
{
    /// <summary>
    /// Provides a generic abstraction for caching operations that can be implemented
    /// using in-memory or distributed providers such as Redis.
    /// </summary>
    public interface ICachingRepository
    {
        /// <summary>
        /// Stores a value in the cache with an optional expiration time.
        /// </summary>
        /// <typeparam name="T">The type of the value to store.</typeparam>
        /// <param name="key">The cache key used to identify the entry.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="expiry">Optional expiration time (TTL).</param>
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);

        /// <summary>
        /// Retrieves a cached value by its key.
        /// </summary>
        /// <typeparam name="T">The expected type of the cached value.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <returns>The cached value or <c>null</c> if not found.</returns>
        Task<T?> GetAsync<T>(string key);

        /// <summary>
        /// Retrieves a cached value or creates it using the provided factory function if not found.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="factory">A function that produces the value if it is not cached.</param>
        /// <param name="expiry">Optional expiration time for the newly cached value.</param>
        /// <returns>The existing or newly created cached value.</returns>
        Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null);

        /// <summary>
        /// Removes a cached entry by its key.
        /// </summary>
        /// <typeparam name="T">The type of the cached value.</typeparam>
        /// <param name="key">The cache key.</param>
        Task RemoveAsync<T>(string key);

        /// <summary>
        /// Determines whether a cached entry exists for the specified key.
        /// </summary>
        /// <typeparam name="T">The type of the cached value.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <returns><c>true</c> if the key exists in the cache; otherwise, <c>false</c>.</returns>
        Task<bool> ExistsAsync<T>(string key);

        /// <summary>
        /// Executes a query that retrieves a collection from cache or from the provided loader if missing.
        /// Optionally filters the cached or loaded results and stores them with an optional TTL.
        /// </summary>
        /// <typeparam name="T">The type of objects being cached.</typeparam>
        /// <param name="predicate">The filter to apply to cached or loaded data.</param>
        /// <param name="loader">A factory function to load data if not available in cache.</param>
        /// <param name="ttl">Optional time-to-live for cached data.</param>
        /// <param name="cacheKey">Optional cache key; if not provided, one should be derived internally.</param>
        /// <returns>A filtered collection of cached or freshly loaded items.</returns>
        Task<ICollection<T>> GroupQuery<T>(
            Func<T, bool> predicate,
            Func<Task<ICollection<T>>> loader,
            TimeSpan? ttl = null,
            string? cacheKey = null);
    }
}
