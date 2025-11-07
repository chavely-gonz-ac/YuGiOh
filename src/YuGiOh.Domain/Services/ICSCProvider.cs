using YuGiOh.Domain.DTOs;
using YuGiOh.Domain.Models;

namespace YuGiOh.Domain.Services
{
    /// <summary>
    /// Provides country, state, city, and street type data from an external or local source.
    /// Abstracts access to geographical information services.
    /// </summary>
    public interface ICSCProvider
    {
        /// <summary>
        /// Retrieves all available countries.
        /// </summary>
        /// <returns>A collection of country DTOs.</returns>
        Task<ICollection<Country>> GetAllCountriesAsync();

        /// <summary>
        /// Retrieves the states associated with the specified country.
        /// </summary>
        /// <param name="countryIso2">The ISO2 code of the country.</param>
        /// <returns>A collection of state DTOs for the given country.</returns>
        Task<ICollection<State>> GetStatesByCountryAsync(string countryIso2);

        /// <summary>
        /// Retrieves the cities within a specific state and country.
        /// </summary>
        /// <param name="countryIso2">The ISO2 code of the country.</param>
        /// <param name="stateIso2">The ISO2 code of the state.</param>
        /// <returns>A collection of city DTOs for the given state.</returns>
        Task<ICollection<City>> GetCitiesByStateAsync(string countryIso2, string stateIso2);

        /// <summary>
        /// Retrieves available street types (e.g., "Street", "Avenue", "Road").
        /// </summary>
        /// <returns>A collection of street type domain entities.</returns>
        Task<ICollection<StreetType>> GetStreetTypesAsync();
    }
}
