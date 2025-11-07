using Microsoft.Extensions.Logging;

namespace YuGiOh.Infrastructure.Seeding.Tools
{
    public interface ISeeder
    {
        Task SeedAsync();
    }
    public class SeedTools<T> : ISeeder
    {
        public Func<Task<IEnumerable<T>>> Provide { get; }
        public Func<T, Task<bool>> Filter { get; }
        public Func<IEnumerable<T>, Task> Save { get; }
        private readonly ILogger<SeedTools<T>> _logger;

        public SeedTools
        (
            Func<Task<IEnumerable<T>>> provide,
            Func<T, Task<bool>> filter,
            Func<IEnumerable<T>, Task> save,
            ILogger<SeedTools<T>> logger
        )
        {
            Provide = provide;
            Filter = filter;
            Save = save;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SeedAsync()
        {
            {

                var entities = await Provide();

                var newEntities = new List<T>();
                foreach (var entity in entities)
                {
                    bool exists = await Filter(entity);
                    if (!exists)
                        newEntities.Add(entity);
                }

                if (newEntities.Count > 0)
                {
                    await Save(newEntities);
                    _logger.LogInformation($"🌱 Seeded {newEntities.Count} new {typeof(T).Name}(s).");
                }
                else
                {
                    _logger.LogInformation($"📭 No new {typeof(T).Name}(s) to seed.");
                }
            }
        }
    }
}