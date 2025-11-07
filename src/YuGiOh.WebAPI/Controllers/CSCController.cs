using MediatR;
using Microsoft.AspNetCore.Mvc;
using YuGiOh.Domain.DTOs;
using YuGiOh.Domain.Models;
using YuGiOh.Domain.Services;

namespace YuGiOh.WebAPI.Controllers
{
    /// <summary>
    /// Provides endpoints for retrieving country, state, city, and street type data
    /// from the CSC (Country-State-City) service.
    /// </summary>
    /// <remarks>
    /// This controller acts as a simple façade between the Web API layer and the
    /// <see cref="ICSCProvider"/> service, which handles data retrieval and caching.
    /// </remarks>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CSCController : APIControllerBase
    {
        private readonly ICSCProvider _cscProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="CSCController"/> class.
        /// </summary>
        /// <param name="mediator">The MediatR mediator instance used for command/query dispatching.</param>
        /// <param name="cscProvider">The service provider responsible for country, state, and city data retrieval.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="mediator"/> or <paramref name="cscProvider"/> is null.
        /// </exception>
        public CSCController(
            IMediator mediator,
            ICSCProvider cscProvider) : base(mediator)
        {
            _cscProvider = cscProvider ?? throw new ArgumentNullException(nameof(cscProvider));
        }

        /// <summary>
        /// Retrieves all countries supported by the CSC service.
        /// </summary>
        /// <returns>A list of <see cref="Country"/> entities.</returns>
        /// <response code="200">Successfully retrieved all countries.</response>
        [HttpGet]
        [ProducesResponseType(typeof(ICollection<Country>), 200)]
        public async Task<IActionResult> GetCountries()
        {
            ICollection<Country> result = await _cscProvider.GetAllCountriesAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves all states belonging to a specific country.
        /// </summary>
        /// <param name="countryIso2">The ISO2 code of the country (e.g., "US", "MX", "JP").</param>
        /// <returns>A list of <see cref="State"/> entities.</returns>
        /// <response code="200">Successfully retrieved all states for the specified country.</response>
        /// <response code="400">Returned when the country code is invalid or missing.</response>
        [HttpGet("{countryIso2}")]
        [ProducesResponseType(typeof(ICollection<State>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetStates(string countryIso2)
        {
            if (string.IsNullOrWhiteSpace(countryIso2))
                return BadRequest("Country ISO2 code is required.");

            var result = await _cscProvider.GetStatesByCountryAsync(countryIso2);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves all cities belonging to a specific state within a given country.
        /// </summary>
        /// <param name="countryIso2">The ISO2 code of the country (e.g., "US").</param>
        /// <param name="stateIso2">The ISO2 code of the state or region (e.g., "CA").</param>
        /// <returns>A list of <see cref="City"/> entities.</returns>
        /// <response code="200">Successfully retrieved all cities for the specified state.</response>
        /// <response code="400">Returned when either ISO2 code is invalid or missing.</response>
        [HttpGet("{countryIso2}/{stateIso2}")]
        [ProducesResponseType(typeof(ICollection<City>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetCities(string countryIso2, string stateIso2)
        {
            if (string.IsNullOrWhiteSpace(countryIso2) || string.IsNullOrWhiteSpace(stateIso2))
                return BadRequest("Both country and state ISO2 codes are required.");

            var result = await _cscProvider.GetCitiesByStateAsync(countryIso2, stateIso2);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves all street types available in the system (e.g., Avenue, Boulevard, Street).
        /// </summary>
        /// <returns>A list of <see cref="StreetType"/> entities.</returns>
        /// <response code="200">Successfully retrieved all street types.</response>
        [HttpGet]
        [ProducesResponseType(typeof(ICollection<StreetType>), 200)]
        public async Task<IActionResult> GetStreetTypes()
        {
            var result = await _cscProvider.GetStreetTypesAsync();
            return Ok(result);
        }
    }
}
