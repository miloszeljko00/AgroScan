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

public class GetRecommendationForDiseaseQueryHandler : IRequestHandler<GetRecommendationForDiseaseQuery, IReadOnlyCollection<AgroChemical>>
{
    private readonly IApplicationDbContext _context;
    private IGraph _graph;
    private ISparqlQueryProcessor _sparqlQueryProcessor;

    public GetRecommendationForDiseaseQueryHandler(IApplicationDbContext context, IGraph graph, ISparqlQueryProcessor sparqlQueryProcessor)
    {
        _context = context;
        _graph = graph;
        _sparqlQueryProcessor = sparqlQueryProcessor;
    }

    public async Task<IReadOnlyCollection<AgroChemical>> Handle(GetRecommendationForDiseaseQuery request, CancellationToken cancellationToken)
    {
        string sparqlQuery = @"
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
                agro:diseaseName """ + request.DiseaseName + @""" ;
                agro:belongsToDiseaseType ?diseaseType .
        }
    }
}
        ";
        var results = ExecuteSparqlQuery(_graph, _sparqlQueryProcessor, sparqlQuery);

        var agroChemicals = new List<AgroChemical>();
        foreach (var result in results)
        {
            string? agroChemicalName = GetValue(result["agroChemicalName"]);
            string? manufacturerName = GetValue(result["manufacturerName"]);
            string? representativeName = GetValue(result["representativeName"]);

            agroChemicals.Add(new AgroChemical()
            {
                Name = agroChemicalName ?? "",
                Manufacturer = manufacturerName ?? "",
                Representative = representativeName ?? ""
            });
        }
        return agroChemicals;
    }

    private static SparqlResultSet ExecuteSparqlQuery(IGraph graph, ISparqlQueryProcessor queryProcessor, string query)
    {
        var sparqlQuery = new SparqlQueryParser().ParseFromString(query);
        return queryProcessor.ProcessQuery(sparqlQuery) as SparqlResultSet;
    }
    private string? GetValue(INode node)
    {
        if (node != null && node.NodeType == NodeType.Literal)
        {
            var literalNode = (ILiteralNode)node;
            return literalNode.Value;
        }

        return null;
    }
}