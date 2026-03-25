using System.Text.Json;
using JobAggregator.Application.Options;
using JobAggregator.Domain.Entities;
using JobAggregator.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JobAggregator.Infrastructure.Scrapers;

public class AdzunaJobSource : IJobSource
{
    private readonly HttpClient _httpClient;
    private readonly AdzunaOptions _options;
    private readonly ILogger<AdzunaJobSource> _logger;

    public string SourceName => "Adzuna";

    public AdzunaJobSource(
        HttpClient httpClient,
        IOptions<AdzunaOptions> options,
        ILogger<AdzunaJobSource> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<IEnumerable<JobOffer>> FetchJobsAsync(
        CancellationToken cancellationToken = default)
    {
        var url = $"https://api.adzuna.com/v1/api/jobs/{_options.Country}/search/1" +
                  $"?app_id={_options.AppId}" +
                  $"&app_key={_options.AppKey}" +
                  $"&results_per_page=50" +
                  $"&content-type=application/json";

        try
        {
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            var json = System.Text.Encoding.UTF8.GetString(bytes);
            var root = JsonSerializer.Deserialize<AdzunaResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (root?.Results is null)
                return Enumerable.Empty<JobOffer>();

            return root.Results.Select(r => new JobOffer
            {
                ExternalId = r.Id,
                Title = r.Title ?? string.Empty,
                Company = r.Company?.DisplayName ?? "Nieznana firma",
                Location = r.Location?.DisplayName ?? string.Empty,
                City = r.Location?.Area?.LastOrDefault(),  // ostatni element = miasto
                Category = r.Category?.Label,
                Description = r.Description,
                SalaryMin = r.SalaryMin,
                SalaryMax = r.SalaryMax,
                Currency = "PLN",
                SourceName = SourceName,
                SourceUrl = r.Redirect_Url ?? string.Empty,
                PublishedAt = r.Created
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania ofert z Adzuna");
            return Enumerable.Empty<JobOffer>();
        }
    }
}

// Modele odpowiedzi Adzuna API
file class AdzunaResponse
{
    public List<AdzunaJob>? Results { get; set; }
}

file class AdzunaJob
{
    public string Id { get; set; } = string.Empty;
    public string? Title { get; set; }
    public AdzunaCompany? Company { get; set; }
    public AdzunaLocation? Location { get; set; }
    public AdzunaCategory? Category { get; set; }
    public string? Description { get; set; }
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public string? Redirect_Url { get; set; }
    public DateTime? Created { get; set; }
}

file class AdzunaCategory
{
    public string? Label { get; set; }
}

file class AdzunaCompany
{
    public string? DisplayName { get; set; }
}

file class AdzunaLocation
{
    public string? DisplayName { get; set; }
    public List<string>? Area { get; set; }
}