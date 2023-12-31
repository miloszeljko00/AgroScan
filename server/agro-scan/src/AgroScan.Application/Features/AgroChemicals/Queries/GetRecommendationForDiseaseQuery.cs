﻿using AgroScan.Application.Common.Interfaces;
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
public class GetRecommendationForDiseaseQuery : IRequest<IReadOnlyCollection<AgroChemical>>
{
    public string DiseaseUri { get; set; } = string.Empty;
}

public class GetRecommendationForDiseaseQueryHandler(IOntologyService ontologyService) : IRequestHandler<GetRecommendationForDiseaseQuery, IReadOnlyCollection<AgroChemical>>
{
    private readonly IOntologyService _ontologyService = ontologyService;

    public async Task<IReadOnlyCollection<AgroChemical>> Handle(GetRecommendationForDiseaseQuery request, CancellationToken cancellationToken)
    {
        var agroChemicals = new List<AgroChemical>();
        string sparqlQuery = BuildQuery(request.DiseaseUri);
        var results = _ontologyService.ExecuteSparqlQuery(sparqlQuery);

        if (results is null) return agroChemicals;

        foreach (var result in results)
        {
            string? agroChemicalUri = result["agroChemical"].ToString();
            string? agroChemicalName = _ontologyService.GetValue(result["agroChemicalName"]);
            string? manufacturerName = _ontologyService.GetValue(result["manufacturerName"]);
            string? representativeName = _ontologyService.GetValue(result["representativeName"]);

            agroChemicals.Add(new AgroChemical()
            {
                Uri = agroChemicalUri,
                Name = agroChemicalName ?? "",
                Manufacturer = manufacturerName ?? "",
                Representative = representativeName ?? ""
            });
        }
        return agroChemicals;
    }

    private static string BuildQuery(string diseaseUri)
    {
        return $@"
        PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>
        PREFIX agro: <http://agroscan.com/ontology/>

        SELECT ?agroChemical ?agroChemicalName ?manufacturerName ?representativeName
        WHERE {{
            ?agroChemical rdf:type agro:AgroChemical ;
                agro:contains ?agroChemicalActiveMaterial ;
                agro:belongsToAgroChemicalType ?agroChemicalType .

            ?agroChemicalType rdf:type agro:AgroChemicalType ;
                agro:prevents ?diseaseType .

            ?agroChemical agro:agroChemicalName ?agroChemicalName ;
                agro:agroChemicalManufacturer ?manufacturerName ;
                agro:agroChemicalRepresentative ?representativeName .

            ?agroChemicalActiveMaterial rdf:type agro:AgroChemicalActiveMaterial ;
                agro:isActiveMaterial ?activeMaterial .

            ?activeMaterial rdf:type agro:ActiveMaterial ;
                agro:cures <{diseaseUri}> .

            <{diseaseUri}> rdf:type agro:Disease ;
                agro:belongsToDiseaseType ?diseaseType .
        }}";
    }


}