using System.Text.Json.Serialization;

namespace YuGiOh.Domain.DTOs
{
    /// <summary>
    /// Represents a country returned by the CSC (Country–State–City) API.
    /// </summary>
    public class Country
    {
        /// <summary>
        /// Gets or sets the ISO 3166-1 alpha-2 country code (e.g., "US", "MX", "JP").
        /// </summary>
        [JsonPropertyName("iso2")]
        public required string Iso2 { get; set; }

        /// <summary>
        /// Gets or sets the human-readable name of the country.
        /// </summary>
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        /// <summary>
        /// Returns a compact string representation of the country in the format "ISO2:Name".
        /// </summary>
        public override string ToString() => $"{Iso2}:{Name}";
    }

    /// <summary>
    /// Represents a state or province returned by the CSC API.
    /// </summary>
    public class State
    {
        /// <summary>
        /// Gets or sets the ISO 3166-2 state code (e.g., "CA", "NY").
        /// </summary>
        [JsonPropertyName("iso2")]
        public required string Iso2 { get; set; }

        /// <summary>
        /// Gets or sets the human-readable name of the state or province.
        /// </summary>
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        /// <summary>
        /// Gets or sets the ISO2 code of the country this state belongs to.
        /// </summary>
        [JsonPropertyName("country_code")]
        public required string CountryIso2 { get; set; }

        /// <summary>
        /// Returns a compact string representation of the state in the format "CountryIso2:Iso2:Name".
        /// </summary>
        public override string ToString() => $"{CountryIso2}:{Iso2}:{Name}";
    }

    /// <summary>
    /// Represents a city returned by the CSC API.
    /// </summary>
    public class City
    {
        /// <summary>
        /// Gets or sets the name of the city.
        /// </summary>
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        /// <summary>
        /// Gets or sets the ISO2 code of the state this city belongs to.
        /// </summary>
        [JsonPropertyName("state_code")]
        public required string StateIso2 { get; set; }

        /// <summary>
        /// Gets or sets the ISO2 code of the country this city belongs to.
        /// </summary>
        [JsonPropertyName("country_code")]
        public required string CountryIso2 { get; set; }

        /// <summary>
        /// Returns a compact string representation of the city in the format "CountryIso2:StateIso2:Name".
        /// </summary>
        public override string ToString() => $"{CountryIso2}:{StateIso2}:{Name}";
    }
}
