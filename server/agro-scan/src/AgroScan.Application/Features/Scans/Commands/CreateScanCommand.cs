using AgroScan.Application.Common.Interfaces;
using AgroScan.Application.Common.Security;
using AgroScan.Application.Features.Diseases.Queries;
using AgroScan.Application.Features.Plants.Queries;
using AgroScan.Core.Constants;
using AgroScan.Core.Entities;

namespace AgroScan.Application.Features.Scans.Queries;

public class CreateScanCommandRequest : IRequest<Scan>
{
    public string UserId { get; set; } = string.Empty;
    public string ImageBase64 { get; set; } = string.Empty;
}

public class CreateScanCommandHandler(IApplicationDbContext context, ISender mediator) : IRequestHandler<CreateScanCommandRequest, Scan>
{
    private readonly IApplicationDbContext _context = context;
    private readonly ISender _mediator = mediator;

    public async Task<Scan> Handle(CreateScanCommandRequest request, CancellationToken cancellationToken)
    {
        var plant = await _mediator.Send(new DeterminePlantQueryRequest 
        {
            ImageBase64 = request.ImageBase64
        }, cancellationToken);

        var disease = await _mediator.Send(new DetermineDiseaseQueryRequest
        {
            ImageBase64 = request.ImageBase64,
            PlantUri = plant.Uri
        }, cancellationToken);

        var scan = new Scan
        {
            UserId = Guid.Parse(request.UserId),
            ImageBase64 = request.ImageBase64,
            CreatedAt = DateTime.UtcNow,
            PlantUri = plant?.Uri ?? string.Empty,
            PlantName = plant?.Name ?? string.Empty,
            DiseaseUri = disease?.Uri ?? null,
            DiseaseName = disease?.Name ?? null,
        };

        await _context.Scans.AddAsync(scan, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return scan;
    }



}