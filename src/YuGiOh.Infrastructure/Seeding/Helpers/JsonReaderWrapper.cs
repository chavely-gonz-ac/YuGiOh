using System.Text.Json;

namespace YuGiOh.Infrastructure.Seeding.Helpers
{
    public static class JsonReaderWrapper
    {
        public static async Task<IEnumerable<T>> ResolveDataFromJson<T>(string? path = null)
        {
            var basePath = AppContext.BaseDirectory;
            string? jsonPath;
            if (path == null)
            {
                jsonPath = Path.Combine(basePath, $"{typeof(T).Name}.json");
            }
            else jsonPath = Path.Combine(basePath, $"{path}.json");

            if (!File.Exists(jsonPath))
            {
                Console.WriteLine($"⚠️ JSON file not found: {jsonPath}");
                return Array.Empty<T>();
            }

            var json = await File.ReadAllTextAsync(jsonPath);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var entities = JsonSerializer.Deserialize<IEnumerable<T>>(json, options) ?? Array.Empty<T>();

            return entities;
            // use case: 
            // var data = await JsonReaderWrapper.ResolveDataFromJson<T>();
            // foreach (var element in data)
            // {
            //     Console.WriteLine(element);
            // }
        }
    }
}