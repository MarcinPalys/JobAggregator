using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JobAggregator.Infrastructure.BackgroundJobs;

public class JobFetcherService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<JobFetcherService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromHours(1);

    public JobFetcherService(
        IServiceScopeFactory scopeFactory,
        ILogger<JobFetcherService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("JobFetcherService uruchomiony");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Rozpoczynam pobieranie ofert: {Time}", DateTimeOffset.Now);

                using var scope = _scopeFactory.CreateScope();
                var orchestrator = scope.ServiceProvider
                    .GetRequiredService<FetchOrchestrator>();

                await orchestrator.FetchAllSourcesAsync(stoppingToken);
                _logger.LogInformation("Pobieranie zakończone");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w JobFetcherService");
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }
}