using System.ComponentModel.DataAnnotations;
using YuGiOh.Domain.Exceptions;

namespace YuGiOh.Infrastructure.CSCService
{
    /// <summary>
    /// Represents configuration settings for the CSC (Card Search/Collector Service) integration.
    /// This includes API access credentials, endpoint configuration, and caching defaults.
    /// </summary>
    public class CSCOptions
    {
        /// <summary>
        /// Gets or sets the API key used to authenticate requests to the CSC service.
        /// </summary>
        /// <exception cref="APIException">
        /// Thrown when the property value is null, empty, or consists only of whitespace.
        /// </exception>
        [Required]
        public required string APIKey
        {
            get => _apiKey;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw APIException.BadRequest("CSC API key cannot be null or empty.");
                _apiKey = value;
            }
        }
        private string _apiKey = string.Empty;

        /// <summary>
        /// Gets or sets the base endpoint URL for the CSC API.
        /// </summary>
        /// <exception cref="APIException">
        /// Thrown when the property value is null, empty, or consists only of whitespace.
        /// </exception>
        [Required]
        public required string Endpoint
        {
            get => _endpoint;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw APIException.BadRequest("CSC API endpoint cannot be null or empty.");
                _endpoint = value;
            }
        }
        private string _endpoint = string.Empty;

        /// <summary>
        /// Gets or sets the default number of hours to cache CSC API responses.
        /// Defaults to 4 hours.
        /// </summary>
        /// <exception cref="APIException">
        /// Thrown when an invalid (negative or zero) cache duration is assigned.
        /// </exception>
        public double DefaultCacheHours
        {
            get => _defaultCacheHours;
            set
            {
                if (value <= 0)
                    throw APIException.BadRequest("Default cache duration must be greater than zero hours.");
                _defaultCacheHours = value;
            }
        }
        private double _defaultCacheHours = 4;
    }
}
