using System.Text.Json;
using System.Text.Json.Serialization;
using JobAggregator.Domain.Entities;
using JobAggregator.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace JobAggregator.Infrastructure.Scrapers;

public class JustJoinJobSource : IJobSource
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<JustJoinJobSource> _logger;

    public string SourceName => "JustJoin.it";

    public JustJoinJobSource(HttpClient httpClient, ILogger<JustJoinJobSource> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IEnumerable<JobOffer>> FetchJobsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            //var url = "https://api.justjoin.it/v2/user-panel/offers?page=1&pageSize=100&sortBy=published&sortOrder=DESC";
            //var url = "https://api.justjoin.it/v2/user-panel/offers?page=1&pageSize=100&sortBy=publishedAt&sortOrder=DESC";
            var url = "https://api.justjoin.it/offers";
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            var json = System.Text.Encoding.UTF8.GetString(bytes);

            var root = JsonSerializer.Deserialize<JustJoinResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (root?.Data is null)
                return Enumerable.Empty<JobOffer>();

            return root.Data.Select(r => new JobOffer
            {
                ExternalId = r.Slug ?? r.Id ?? Guid.NewGuid().ToString(),
                Title = r.Title ?? string.Empty,
                Company = r.CompanyName ?? "Nieznana firma",
                Location = r.City ?? string.Empty,
                City = r.City,
                Category = r.MainCategory,
                Description = r.Body,
                SalaryMin = r.EmploymentTypes?.FirstOrDefault()?.FromPln,
                SalaryMax = r.EmploymentTypes?.FirstOrDefault()?.ToPln,
                Currency = "PLN",
                SourceName = SourceName,
                SourceUrl = $"https://justjoin.it/offers/{r.Slug}",
                PublishedAt = r.PublishedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania ofert z JustJoin.it");
            return Enumerable.Empty<JobOffer>();
        }
    }
}

file class JustJoinResponse
{
    public List<JustJoinJob>? Data { get; set; }
}

file class JustJoinJob
{
    public string? Id { get; set; }
    public string? Slug { get; set; }
    public string? Title { get; set; }
    public string? CompanyName { get; set; }
    public string? City { get; set; }
    public string? MainCategory { get; set; }
    public string? Body { get; set; }
    public DateTime? PublishedAt { get; set; }
    public List<JustJoinEmploymentType>? EmploymentTypes { get; set; }
}

file class JustJoinEmploymentType
{
    public decimal? FromPln { get; set; }
    public decimal? ToPln { get; set; }
}