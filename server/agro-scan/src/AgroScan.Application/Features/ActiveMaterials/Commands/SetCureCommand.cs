using AgroScan.Application.Common.Interfaces;

namespace AgroScan.Application.Features.ActiveMaterials.Commands;
public record SetCureCommandRequest : IRequest<bool>
{
    public string ActiveMaterialUri { get; set; } = string.Empty;
    public string DiseaseUri { get; set; } = string.Empty;
}

public class SetCureCommandHandler(IOntologyService ontologyService) : IRequestHandler<SetCureCommandRequest, bool>
{
    private readonly IOntologyService ontologyService = ontologyService;

    public async Task<bool> Handle(SetCureCommandRequest request, CancellationToken cancellationToken)
    {
        return ontologyService.SetActiveMaterialAsCureForDisease(request.ActiveMaterialUri, request.DiseaseUri);
    }
}
