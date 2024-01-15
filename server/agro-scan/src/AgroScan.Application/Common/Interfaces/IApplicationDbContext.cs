using AgroScan.Core.Entities;

namespace AgroScan.Application.Common.Interfaces;
public interface IApplicationDbContext
{
    DbSet<Scan> Scans { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
