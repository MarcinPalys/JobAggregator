using JobAggregator.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace JobAggregator.Infrastructure.BackgroundJobs;

public class FetchOrchestrator
{
    private readonly IEnumerable<IJobSource> _sources;
    private readonly IJobRepository _repository;
    private readonly ILogger<FetchOrchestrator> _logger;

    public FetchOrchestrator(
        IEnumerable<IJobSource> sources,
        IJobRepository repository,
        ILogger<FetchOrchestrator> logger)
    {
        _sources = sources;
        _repository = repository;
        _logger = logger;
    }

    public async Task FetchAllSourcesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var source in _sources)
        {
            try
            {
                _logger.LogInformation("Pobieranie ofert z {Source}", source.SourceName);
                var offers = await source.FetchJobsAsync(cancellationToken);
                var jobOffers = offers.ToList();
                await _repository.UpsertAsync(jobOffers, cancellationToken);
                _logger.LogInformation("Zapisano {Count} ofert z {Source}",
                    jobOffers.Count, source.SourceName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas pobierania z {Source}", source.SourceName);
            }
        }
    }
}