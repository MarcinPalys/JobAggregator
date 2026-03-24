using JobAggregator.Domain.Entities;
using JobAggregator.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobAggregator.Infrastructure.Persistence
{
    public class JobRepository : IJobRepository
    {
        private readonly AppDbContext _context;

        public JobRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<JobOffer>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.JobOffers
                .OrderByDescending(j => j.FetchedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<JobOffer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.JobOffers
                .FirstOrDefaultAsync(j => j.Id == id, cancellationToken);
        }

        public async Task UpsertAsync(IEnumerable<JobOffer> offers, CancellationToken cancellationToken = default)
        {
            foreach (var offer in offers)
            {
                var existing = await _context.JobOffers
                    .FirstOrDefaultAsync(j =>
                        j.ExternalId == offer.ExternalId &&
                        j.SourceName == offer.SourceName,
                        cancellationToken);

                if (existing is null)
                {
                    offer.Id = Guid.NewGuid();
                    offer.FetchedAt = DateTime.UtcNow;
                    await _context.JobOffers.AddAsync(offer, cancellationToken);
                }
                else
                {
                    existing.Title = offer.Title;
                    existing.Company = offer.Company;
                    existing.Location = offer.Location;
                    existing.SalaryMin = offer.SalaryMin;
                    existing.SalaryMax = offer.SalaryMax;
                    existing.FetchedAt = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
