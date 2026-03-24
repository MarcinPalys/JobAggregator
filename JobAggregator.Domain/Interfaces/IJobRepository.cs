using JobAggregator.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobAggregator.Domain.Interfaces
{
    public interface IJobRepository
    {
        Task<IEnumerable<JobOffer>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<JobOffer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task UpsertAsync(IEnumerable<JobOffer> offers, CancellationToken cancellationToken = default);
    }
}
