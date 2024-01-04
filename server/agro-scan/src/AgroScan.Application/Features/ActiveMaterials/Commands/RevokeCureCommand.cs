using AgroScan.Application.Common.Interfaces;

namespace AgroScan.Application.Features.ActiveMaterials.Commands;
public record RevokeCureCommandRequest : IRequest<bool>
{
    public string ActiveMaterialUri { get; set; } = string.Empty;
    public string DiseaseUri { get; set; } = string.Empty;
}

public class RevokeCureCommandHandler(IOntologyService ontologyService) : IRequestHandler<RevokeCureCommandRequest, bool>
{
    private readonly IOntologyService ontologyService = ontologyService;

    public async Task<bool> Handle(RevokeCureCommandRequest request, CancellationToken cancellationToken)
    {
        return ontologyService.RevokeActiveMaterialAsCureForDisease(request.ActiveMaterialUri, request.DiseaseUri);
    }
}
