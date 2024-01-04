using AgroScan.Application.Common.Interfaces;
using AgroScan.Core.Entities;

namespace AgroScan.Application.Features.ActiveMaterials.Queries;
public class GetAllActiveMaterialsByDiseaseNameQueryRequest : IRequest<IReadOnlyCollection<ActiveMaterial>>
{
    public string DiseaseName { get; set; } = string.Empty;
}

public class GetAllDiseasesByPlantNameQueryHandler(IOntologyService ontologyService) : IRequestHandler<GetAllActiveMaterialsByDiseaseNameQueryRequest, IReadOnlyCollection<ActiveMaterial>>
{
    private readonly IOntologyService _ontologyService = ontologyService;

    public async Task<IReadOnlyCollection<ActiveMaterial>> Handle(GetAllActiveMaterialsByDiseaseNameQueryRequest request, CancellationToken cancellationToken)
    {
        var activeMaterials = new List<ActiveMaterial>();
        string sparqlQuery = BuildQuery(request.DiseaseName);
        var results = _ontologyService.ExecuteSparqlQuery(sparqlQuery);

        if (results is null) return activeMaterials;

        foreach (var result in results)
        {
            string? activeMaterialName = _ontologyService.GetValue(result["activeMaterialName"]) ?? "";
            activeMaterials.Add(new ActiveMaterial() { Name = activeMaterialName });
        }
        return activeMaterials;
    }

    private static string BuildQuery(string diseaseName)
    {
        return $@"
        PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>
        PREFIX agro: <http://agroscan.com/ontology/>

        SELECT ?activeMaterialName
        WHERE {{
          ?activeMaterial rdf:type agro:ActiveMaterial ;
                          agro:cures ?disease .

          ?disease rdf:type agro:Disease ;
                  agro:diseaseName ""{diseaseName}"" .
      
          ?activeMaterial agro:activeMaterialName ?activeMaterialName .
        }}";
    }



}