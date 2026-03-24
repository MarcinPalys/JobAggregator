using JobAggregator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobAggregator.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<JobOffer> JobOffers => Set<JobOffer>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<JobOffer>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Title).IsRequired().HasMaxLength(300);
                entity.Property(e => e.Company).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Location).HasMaxLength(200);
                entity.Property(e => e.SourceName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.SourceUrl).IsRequired().HasMaxLength(500);
                entity.Property(e => e.ExternalId).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Currency).HasMaxLength(10);

                // unikalny klucz – zapobiega duplikatom
                entity.HasIndex(e => new { e.ExternalId, e.SourceName }).IsUnique();
            });
        }
    }
}
