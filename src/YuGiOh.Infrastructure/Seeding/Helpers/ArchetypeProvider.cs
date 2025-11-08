using System.Net.Http.Json;
using System.Text.Json.Serialization;
using YuGiOh.Domain.Models;

namespace YuGiOh.Infrastructure.Seeding.Helpers
{
    /// <summary>
    /// Provides all Yu-Gi-Oh! archetypes by fetching them once from the public YGIOPRODeck API.
    /// </summary>
    public static class ArchetypesProvider
    {
        private readonly static string ApiUrl = "https://db.ygoprodeck.com/api/v7/archetypes.php";

        public static async Task<IEnumerable<Archetype>> FetchArchetypesFromWebAsync()
        {
            var _httpClient = new HttpClient();
            var response = await _httpClient.GetAsync(ApiUrl);
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadFromJsonAsync<List<DTO>>();
            int i = 0;
            // Clean, distinct, sorted, and convert to Archetype objects
            var archetypes = data
                .Select(dto => dto.archetype_name?.Trim())
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(name => name)
                .Select((name, index) => new Archetype { Name = name })
                .ToList();

            return archetypes;
        }
        private record DTO { public string archetype_name { get; set; } = null!; }
    }
}
