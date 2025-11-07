using YuGiOh.Domain.DTOs;
using YuGiOh.Domain.Models;
using YuGiOh.Domain.Services;
using YuGiOh.Domain.Repositories;
using Microsoft.Extensions.Options;
using Ardalis.Specification;
using YuGiOh.Domain.Exceptions;

namespace YuGiOh.Infrastructure.CSCService
{
    /// <summary>
    /// Provides a unified interface for accessing CSC (Country-State-City) data,
    /// combining remote API calls with caching and local data persistence for performance and resilience.
    /// </summary>
    public class CSCProvider : ICSCProvider
    {
        private readonly CSCLoader _loader;
        private readonly ICachingRepository _cache;
        private readonly IReadRepositoryBase<StreetType> _streetTypeRepository;
        private readonly TimeSpan _ttl;

        /// <summary>
        /// Initializes a new instance of the <see cref="CSCProvider"/> class.
        /// </summary>
        /// <param name="cache">Caching repository instance used to cache API responses.</param>
        /// <param name="loader">The service responsible for making CSC API requests.</param>
        /// <param name="streetTypeRepository">Repository for reading street type data from the database.</param>
        /// <param name="options">Configuration options defining caching TTL and API behavior.</param>
        /// <exception cref="APIException">
        /// Thrown when any dependency is null or configuration options are invalid.
        /// </exception>
        public CSCProvider(
            ICachingRepository cache,
            CSCLoader loader,
            IReadRepositoryBase<StreetType> streetTypeRepository,
            IOptions<CSCOptions> options)
        {
            _cache = cache ?? throw APIException.BadRequest("Caching repository cannot be null.", nameof(cache));
            _loader = loader ?? throw APIException.BadRequest("CSC API loader cannot be null.", nameof(loader));
            _streetTypeRepository = streetTypeRepository ?? throw APIException.BadRequest("StreetType repository cannot be null.", nameof(streetTypeRepository));

            var opts = options?.Value ?? throw APIException.BadRequest("CSC configuration options cannot be null.", nameof(options));

            if (opts.DefaultCacheHours <= 0)
                throw APIException.BadRequest("Default cache duration must be greater than zero hours.");

            _ttl = TimeSpan.FromHours(opts.DefaultCacheHours);
        }

        /// <summary>
        /// Retrieves and caches the list of all countries from the CSC API.
        /// </summary>
        /// <returns>A cached collection of <see cref="Country"/> objects.</returns>
        public Task<ICollection<Country>> GetAllCountriesAsync()
            => _cache.GroupQuery(
                c => true,
                _loader.GetAllCountriesAsync,
                _ttl,
                "countries");

        /// <summary>
        /// Retrieves and caches all states for a specified country.
        /// </summary>
        /// <param name="countryIso2">The ISO2 code of the target country.</param>
        /// <returns>A cached collection of <see cref="State"/> objects for that country.</returns>
        /// <exception cref="APIException">Thrown when <paramref name="countryIso2"/> is null or empty.</exception>
        public Task<ICollection<State>> GetStatesByCountryAsync(string countryIso2)
        {
            if (string.IsNullOrWhiteSpace(countryIso2))
                throw APIException.BadRequest("Country ISO2 code cannot be null or empty.", nameof(countryIso2));

            return _cache.GroupQuery(
                s => true,
                () => _loader.GetStatesByCountryAsync(countryIso2),
                _ttl,
                $"states:{countryIso2}");
        }

        /// <summary>
        /// Retrieves and caches all cities for a given state within a country.
        /// </summary>
        /// <param name="countryIso2">The ISO2 code of the country.</param>
        /// <param name="stateIso2">The ISO2 code of the state.</param>
        /// <returns>A cached collection of <see cref="City"/> objects.</returns>
        /// <exception cref="APIException">
        /// Thrown when <paramref name="countryIso2"/> or <paramref name="stateIso2"/> is null or empty.
        /// </exception>
        public Task<ICollection<City>> GetCitiesByStateAsync(string countryIso2, string stateIso2)
        {
            if (string.IsNullOrWhiteSpace(countryIso2))
                throw APIException.BadRequest("Country ISO2 code cannot be null or empty.", nameof(countryIso2));

            if (string.IsNullOrWhiteSpace(stateIso2))
                throw APIException.BadRequest("State ISO2 code cannot be null or empty.", nameof(stateIso2));

            return _cache.GroupQuery(
                c => true,
                () => _loader.GetCitiesByStateAsync(countryIso2, stateIso2),
                _ttl,
                $"cities:{countryIso2}:{stateIso2}");
        }

        /// <summary>
        /// Retrieves and caches the list of valid street types from the database.
        /// </summary>
        /// <returns>A cached collection of <see cref="StreetType"/> records.</returns>
        public async Task<ICollection<StreetType>> GetStreetTypesAsync()
        {
            return await _cache.GroupQuery<StreetType>(
                s => true,
                async () => await _streetTypeRepository.ListAsync(),
                _ttl,
                "streettypes");
        }
    }
}
