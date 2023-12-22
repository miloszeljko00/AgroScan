using AgroScan.Application.Common.Interfaces;
using AgroScan.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF.Query;
using VDS.RDF;
using VDS.RDF.Parsing;

namespace AgroScan.Application.Features.AgroChemicals.Queries;
public class GetRecommendationForDiseaseQuery: IRequest<IReadOnlyCollection<AgroChemical>>
{
    public string DiseaseName { get; set; }
}

public class GetRecommendationForDiseaseQueryHandler(IApplicationDbContext context, IOntologyService ontologyService) : IRequestHandler<GetRecommendationForDiseaseQuery, IReadOnlyCollection<AgroChemical>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IOntologyService _ontologyService = ontologyService;

    public async Task<IReadOnlyCollection<AgroChemical>> Handle(GetRecommendationForDiseaseQuery request, CancellationToken cancellationToken)
    {
        var agroChemicals = new List<AgroChemical>();
        string sparqlQuery = BuildQuery(request.DiseaseName);
        var results = _ontologyService.ExecuteSparqlQuery(sparqlQuery);

        if (results is null) return agroChemicals;

        foreach (var result in results)
        {
            string? agroChemicalName = _ontologyService.GetValue(result["agroChemicalName"]);
            string? manufacturerName = _ontologyService.GetValue(result["manufacturerName"]);
            string? representativeName = _ontologyService.GetValue(result["representativeName"]);

            agroChemicals.Add(new AgroChemical()
            {
                Name = agroChemicalName ?? "",
                Manufacturer = manufacturerName ?? "",
                Representative = representativeName ?? ""
            });
        }
        return agroChemicals;
    }

    private static string BuildQuery(string diseaseName)
    {
        return @"
PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>
PREFIX agro: <http://agroscan.com/ontology/>

SELECT ?agroChemicalName ?manufacturerName ?representativeName
WHERE {
    ?agroChemical rdf:type agro:AgroChemical ;
                agro:contains ?activeMaterial ;
                agro:belongsToAgroChemicalType ?agroChemicalType .

    ?agroChemicalType rdf:type agro:AgroChemicalType ;
                    agro:prevents ?diseaseType .

    ?agroChemical agro:agroChemicalName ?agroChemicalName ;
                agro:agroChemicalManufacturer ?manufacturerName ;
                agro:agroChemicalRepresentative ?representativeName .

    # Subquery to get ActiveMaterials that cure a specific disease and their disease types
    {
    SELECT ?activeMaterial ?disease ?diseaseType
    WHERE {
        ?activeMaterial rdf:type agro:ActiveMaterial ;
                        agro:cures ?disease .

        ?disease rdf:type agro:Disease ;
                agro:diseaseName """ + diseaseName + @""" ;
                agro:belongsToDiseaseType ?diseaseType .
        }
    }
}
        ";
    }

}