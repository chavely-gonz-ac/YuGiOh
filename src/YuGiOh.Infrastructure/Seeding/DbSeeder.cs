using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using YuGiOh.Infrastructure.Seeding.Tools;

namespace YuGiOh.Infrastructure.Seeding
{
    public sealed class DbSeeder : IHostedService, IDisposable
    {
        #region Constructor
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DbSeeder> _logger;
        private Timer? _timer;

        public DbSeeder(IServiceScopeFactory scopeFactory, ILogger<DbSeeder> logger)
        {
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        #endregion

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("🪴 DbSeeder initialized. First run starting...");
            _timer = new Timer(async _ => await RunAsync(), null, TimeSpan.Zero, TimeSpan.FromDays(7));
            return Task.CompletedTask;
        }

        private async Task RunAsync()
        {

            using var scope = _scopeFactory.CreateScope();
            var seedingToolsProvider = scope.ServiceProvider.GetRequiredService<SeedingToolsProvider>();
            try
            {
                _logger.LogInformation("🔄 Starting seeding cycle...");

                for (int i = 0; i < seedingToolsProvider.SeedingTools.Count; i++)
                {
                    if (seedingToolsProvider.SeedingTools[i] is ISeeder tool)
                        await tool.SeedAsync();
                }

                _logger.LogInformation("✅ Seeding cycle completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error during seeding cycle.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("🛑 DbSeeder stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}