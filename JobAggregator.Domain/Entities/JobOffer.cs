using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobAggregator.Domain.Entities
{
    public class JobOffer
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string? City { get; set; }        // nowe
        public string? Category { get; set; }    // nowe
        public string? Description { get; set; }
        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public string? Currency { get; set; }
        public string SourceName { get; set; } = string.Empty;
        public string SourceUrl { get; set; } = string.Empty;
        public string ExternalId { get; set; } = string.Empty;
        public DateTime FetchedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
    }
}
