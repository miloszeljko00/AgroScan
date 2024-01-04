using AgroScan.Application.Common.Interfaces;
using AgroScan.Core.Entities;

namespace AgroScan.Application.Features.ActiveMaterials.Queries;
public class GetAllActiveMaterialsThatCuresDiseaseQueryRequest : IRequest<IReadOnlyCollection<ActiveMaterial>>
{
    public string DiseaseUri { get; set; } = string.Empty;
}

public class GetAllActiveMaterialsThatCuresDiseaseQueryHandler(IOntologyService ontologyService) : IRequestHandler<GetAllActiveMaterialsThatCuresDiseaseQueryRequest, IReadOnlyCollection<ActiveMaterial>>
{
    private readonly IOntologyService _ontologyService = ontologyService;

    public async Task<IReadOnlyCollection<ActiveMaterial>> Handle(GetAllActiveMaterialsThatCuresDiseaseQueryRequest request, CancellationToken cancellationToken)
    {
        var activeMaterials = new List<ActiveMaterial>();
        string sparqlQuery = BuildQuery(request.DiseaseUri);
        var results = _ontologyService.ExecuteSparqlQuery(sparqlQuery);

        if (results is null) return activeMaterials;

        foreach (var result in results)
        {
            string? activeMaterialName = _ontologyService.GetValue(result["activeMaterialName"]) ?? "";
            string activeMaterialUri = result["activeMaterial"].ToString();
            
            activeMaterials.Add(new ActiveMaterial() 
            {
                Uri = activeMaterialUri,
                Name = activeMaterialName 
            });
        }
        return activeMaterials;
    }

    private static string BuildQuery(string diseaseUri)
    {
        return $@"
        PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>
        PREFIX agro: <http://agroscan.com/ontology/>

        SELECT ?activeMaterial ?activeMaterialName
        WHERE {{
            ?activeMaterial rdf:type agro:ActiveMaterial ;
                            agro:cures <{diseaseUri}> .

            <{diseaseUri}> rdf:type agro:Disease .

            ?activeMaterial agro:activeMaterialName ?activeMaterialName .
        }}";
    }




}