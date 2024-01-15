using AgroScan.Application.Common.Interfaces;
using AgroScan.Application.Common.Security;
using AgroScan.Core.Constants;
using AgroScan.Core.Entities;

namespace AgroScan.Application.Features.Plants.Queries;

public class DeterminePlantQueryRequest : IRequest<Plant>
{
    public string ImageBase64 { get; set; } = string.Empty;
}

public class DeterminePlantQueryHandler() : IRequestHandler<DeterminePlantQueryRequest, Plant>
{
    public async Task<Plant> Handle(DeterminePlantQueryRequest request, CancellationToken cancellationToken)
    {
        return new Plant()
        {
            Name = "Tomato",
            Uri = "http://agroscan.com/ontology/tomato"
        };
    }
}