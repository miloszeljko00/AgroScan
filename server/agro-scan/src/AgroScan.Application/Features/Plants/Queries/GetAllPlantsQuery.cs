using AgroScan.Application.Common.Interfaces;
using AgroScan.Application.Common.Security;
using AgroScan.Core.Constants;
using AgroScan.Core.Entities;

namespace AgroScan.Application.Features.Plants.Queries;

public class GetAllPlantsQueryRequest : IRequest<IReadOnlyCollection<Plant>>
{
}

public class GetAllPlantsQueryHandler(IOntologyService ontologyService) : IRequestHandler<GetAllPlantsQueryRequest, IReadOnlyCollection<Plant>>
{
    private readonly IOntologyService _ontologyService = ontologyService;

    public async Task<IReadOnlyCollection<Plant>> Handle(GetAllPlantsQueryRequest request, CancellationToken cancellationToken)
    {
        var plants = new List<Plant>();
        string sparqlQuery = BuildQuery();
        var results = _ontologyService.ExecuteSparqlQuery(sparqlQuery);

        if (results is null) return plants;

        foreach (var result in results)
        {
            string? plantName = _ontologyService.GetValue(result["plantName"]) ?? "";
            string plantUri = result["plant"].ToString();
            plants.Add(new Plant() 
            { 
                Uri = plantUri,
                Name = plantName
            });
        }
        return plants;
    }

    private static string BuildQuery()
    {
        return $@"
        PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>
        PREFIX agro: <http://agroscan.com/ontology/>

        SELECT ?plant ?plantName
        WHERE {{
          ?plant rdf:type agro:Plant ;
                   agro:plantName ?plantName .
        }}";
    }

}