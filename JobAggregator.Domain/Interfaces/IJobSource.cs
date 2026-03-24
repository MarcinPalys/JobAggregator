using JobAggregator.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobAggregator.Domain.Interfaces
{
    public interface IJobSource
    {
        string SourceName { get; }
        Task<IEnumerable<JobOffer>> FetchJobsAsync(CancellationToken cancellationToken = default);
    }
}
