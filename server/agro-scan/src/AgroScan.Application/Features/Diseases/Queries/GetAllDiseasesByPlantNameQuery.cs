using AgroScan.Application.Common.Interfaces;
using AgroScan.Core.Entities;

namespace AgroScan.Application.Features.Diseases.Queries;
public class GetAllDiseasesByPlantNameQueryRequest : IRequest<IReadOnlyCollection<Disease>>
{
    public string PlantName { get; set; } = string.Empty;
}

public class GetAllDiseasesByPlantNameQueryHandler(IOntologyService ontologyService) : IRequestHandler<GetAllDiseasesByPlantNameQueryRequest, IReadOnlyCollection<Disease>>
{
    private readonly IOntologyService _ontologyService = ontologyService;

    public async Task<IReadOnlyCollection<Disease>> Handle(GetAllDiseasesByPlantNameQueryRequest request, CancellationToken cancellationToken)
    {
        var diseases = new List<Disease>();
        string sparqlQuery = BuildQuery(request.PlantName);
        var results = _ontologyService.ExecuteSparqlQuery(sparqlQuery);

        if (results is null) return diseases;

        foreach (var result in results)
        {
            string? diseaseName = _ontologyService.GetValue(result["diseaseName"]) ?? "";
            diseases.Add(new Disease() { Name = diseaseName });
        }
        return diseases;
    }

    private static string BuildQuery(string plantName)
    {
        return $@"
        PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>
        PREFIX agro: <http://agroscan.com/ontology/>

        SELECT ?diseaseName
        WHERE {{
            ?disease rdf:type agro:Disease ;
                    agro:diseaseName ?diseaseName ;
                    agro:infects ?plant .

            ?plant rdf:type agro:Plant ;
                    agro:plantName ""{plantName}"" .
        }}";
    }


}