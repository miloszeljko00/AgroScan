using AgroScan.Application.Common.Interfaces;
using AgroScan.Application.Common.Security;
using AgroScan.Application.Features.Plants.Queries;
using AgroScan.Core.Constants;
using AgroScan.Core.Entities;

namespace AgroScan.Application.Features.Scans.Queries;

public class GetAllScansByUserIdQueryRequest : IRequest<IReadOnlyCollection<Scan>>
{
    public string UserId { get; set; } = string.Empty;
}

public class GetAllPlantsQueryHandler(IApplicationDbContext context) : IRequestHandler<GetAllScansByUserIdQueryRequest, IReadOnlyCollection<Scan>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<IReadOnlyCollection<Scan>> Handle(GetAllScansByUserIdQueryRequest request, CancellationToken cancellationToken)
    {
        return await _context.Scans
            .Where(scan => scan.UserId == Guid.Parse(request.UserId))
            .ToListAsync(cancellationToken);
    }



}