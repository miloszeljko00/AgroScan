﻿using AgroScan.Application.Common.Interfaces;
using AgroScan.Application.Common.Security;
using AgroScan.Core.Constants;
using AgroScan.Core.Entities;

namespace AgroScan.Application.Features.ActiveMaterials.Queries;
public class GetAllActiveMaterialsQueryRequest : IRequest<IReadOnlyCollection<ActiveMaterial>>
{
}

[Authorize(Roles = Roles.User)]
public class GetAllActiveMaterialsQueryHandler(IOntologyService ontologyService) : IRequestHandler<GetAllActiveMaterialsQueryRequest, IReadOnlyCollection<ActiveMaterial>>
{
    private readonly IOntologyService _ontologyService = ontologyService;

    public async Task<IReadOnlyCollection<ActiveMaterial>> Handle(GetAllActiveMaterialsQueryRequest request, CancellationToken cancellationToken)
    {
        var activeMaterials = new List<ActiveMaterial>();
        string sparqlQuery = BuildQuery();
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

    private static string BuildQuery()
    {
        return @"
        PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>
        PREFIX agro: <http://agroscan.com/ontology/>

        SELECT ?activeMaterial ?activeMaterialName
        WHERE {
          ?activeMaterial rdf:type agro:ActiveMaterial ;
                          agro:activeMaterialName ?activeMaterialName .
        }";
    }
}