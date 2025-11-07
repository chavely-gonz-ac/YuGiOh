using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YuGiOh.Domain.DTOs;
using YuGiOh.Domain.Exceptions;

namespace YuGiOh.Infrastructure.CSCService
{
    /// <summary>
    /// Provides methods for fetching hierarchical geographic data (countries, states, cities)
    /// from the external CSC (Country-State-City) API.
    /// </summary>
    /// <remarks>
    /// This service is designed to centralize all CSC API interactions and provide a clean,
    /// strongly-typed interface for retrieving location data, with standardized exception handling.
    /// </remarks>
    public class CSCLoader
    {
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly string _urlBase;
        private readonly HttpClient _http;
        private readonly ILogger<CSCLoader> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CSCLoader"/> class with configured dependencies.
        /// </summary>
        /// <param name="http">The <see cref="HttpClient"/> used for making API requests.</param>
        /// <param name="options">The CSC API configuration options.</param>
        /// <param name="logger">Logger instance for diagnostics and error tracking.</param>
        /// <exception cref="APIException">
        /// Thrown when required dependencies are null or configuration options are invalid.
        /// </exception>
        public CSCLoader(HttpClient http, IOptions<CSCOptions> options, ILogger<CSCLoader> logger)
        {
            var opts = options?.Value ?? throw APIException.BadRequest(
                "CSC configuration options cannot be null.",
                nameof(options));

            _logger = logger ?? throw APIException.BadRequest(
                "Logger instance cannot be null.",
                nameof(logger));

            _http = http ?? throw APIException.BadRequest(
                "HttpClient cannot be null.",
                nameof(http));

            try
            {
                // Set JSON deserialization behavior (case-insensitive)
                _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                // Build base API URL
                _urlBase = $"{opts.Endpoint.TrimEnd('/')}/countries";

                // Add API key to all outgoing requests
                if (!_http.DefaultRequestHeaders.TryAddWithoutValidation("X-CSCAPI-KEY", opts.APIKey))
                    throw APIException.BadRequest("Failed to add CSC API key to request headers.");

                _http.Timeout = TimeSpan.FromSeconds(15);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize CSC API loader.");
                throw APIException.Internal("Error initializing CSC API client.", ex.Message, ex);
            }
        }

        /// <summary>
        /// Retrieves a list of all countries from the CSC API.
        /// </summary>
        /// <returns>A collection of <see cref="Country"/> objects.</returns>
        /// <exception cref="APIException">Thrown if the request fails or the response is invalid.</exception>
        public async Task<ICollection<Country>> GetAllCountriesAsync()
            => await GetFromApiAsync<Country>(_urlBase);

        /// <summary>
        /// Retrieves a list of all states for a specified country.
        /// </summary>
        /// <param name="countryIso2">The ISO2 code of the country.</param>
        /// <returns>A collection of <see cref="State"/> objects.</returns>
        /// <exception cref="APIException">Thrown when <paramref name="countryIso2"/> is invalid or the request fails.</exception>
        public async Task<ICollection<State>> GetStatesByCountryAsync(string countryIso2)
        {
            if (string.IsNullOrWhiteSpace(countryIso2))
                throw APIException.BadRequest("Country ISO2 code cannot be null or empty.", nameof(countryIso2));

            var result = await GetFromApiAsync<State>($"{_urlBase}/{countryIso2}/states");

            // Enrich data with parent identifier
            foreach (var s in result)
                s.CountryIso2 = countryIso2;

            return result;
        }

        /// <summary>
        /// Retrieves a list of all cities for a given state within a country.
        /// </summary>
        /// <param name="countryIso2">The ISO2 code of the country.</param>
        /// <param name="stateIso2">The ISO2 code of the state.</param>
        /// <returns>A collection of <see cref="City"/> objects.</returns>
        /// <exception cref="APIException">Thrown when parameters are invalid or the request fails.</exception>
        public async Task<ICollection<City>> GetCitiesByStateAsync(string countryIso2, string stateIso2)
        {
            if (string.IsNullOrWhiteSpace(countryIso2))
                throw APIException.BadRequest("Country ISO2 code cannot be null or empty.", nameof(countryIso2));

            if (string.IsNullOrWhiteSpace(stateIso2))
                throw APIException.BadRequest("State ISO2 code cannot be null or empty.", nameof(stateIso2));

            var result = await GetFromApiAsync<City>(
                $"{_urlBase}/{countryIso2}/states/{stateIso2}/cities");

            foreach (var c in result)
            {
                c.CountryIso2 = countryIso2;
                c.StateIso2 = stateIso2;
            }

            return result;
        }

        /// <summary>
        /// Sends an HTTP GET request to the specified CSC API endpoint and deserializes the JSON response.
        /// </summary>
        /// <typeparam name="T">The target model type to deserialize.</typeparam>
        /// <param name="url">The API endpoint to call.</param>
        /// <returns>A collection of deserialized objects of type <typeparamref name="T"/>.</returns>
        /// <exception cref="APIException">
        /// Thrown when the API request fails, times out, or returns invalid JSON data.
        /// </exception>
        private async Task<ICollection<T>> GetFromApiAsync<T>(string url)
        {
            try
            {
                var data = await _http.GetFromJsonAsync<List<T>>(url, _jsonOptions).ConfigureAwait(false);
                _logger.LogDebug("Fetched {Count} {Type} items from {Url}", data?.Count ?? 0, typeof(T).Name, url);
                return data ?? new List<T>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error when calling {Url}", url);
                throw APIException.Internal($"HTTP request to CSC API failed for {url}", ex.Message, ex);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogWarning(ex, "Request timeout for {Url}", url);
                throw APIException.Internal($"CSC API request timed out for {url}", ex.Message, ex);
            }
            catch (NotSupportedException ex)
            {
                _logger.LogError(ex, "Unsupported content type received from {Url}", url);
                throw APIException.Internal($"Invalid or unsupported content received from {url}", ex.Message, ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse CSC API response from {Url}", url);
                throw APIException.Internal($"Invalid JSON format received from {url}", ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error calling CSC API at {Url}", url);
                throw APIException.Internal("Unexpected error when fetching data from CSC API.", ex.Message, ex);
            }
        }
    }
}
