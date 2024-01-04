using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF;
using AgroScan.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using AgroScan.Application.Features.AgroChemicals.Commands;
using VDS.RDF.Ontology;
using VDS.RDF.Writing;
using AngleSharp.Io;
using OfficeOpenXml;
using System.Text;

namespace AgroScan.Infrastructure.Ontology;
internal class OntologyService(ISparqlQueryProcessor sparqlQueryProcessor, IGraph graph, IConfiguration configuration) : IOntologyService
{
    private ISparqlQueryProcessor _sparqlQueryProcessor = sparqlQueryProcessor;
    private readonly IGraph _graph = graph;
    private readonly string _ontologyFilePath = configuration["OntologyFilePath"] ?? "";

    public SparqlResultSet? ExecuteSparqlQuery(string query)
    {
        var sparqlQuery = new SparqlQueryParser().ParseFromString(query);
        return _sparqlQueryProcessor.ProcessQuery(sparqlQuery) as SparqlResultSet;
    }
    public string? GetValue(INode node)
    {
        if (node != null && node.NodeType == NodeType.Literal)
        {
            var literalNode = (ILiteralNode)node;
            return literalNode.Value;
        }

        return null;
    }
    public bool RevokeActiveMaterialAsCureForDisease(string activeMaterialUri, string diseaseUri)
    {
        try
        {
            var activeMaterialNode = _graph.CreateUriNode(new Uri(activeMaterialUri));
            var curesNode = _graph.CreateUriNode(new Uri("http://agroscan.com/ontology/cures"));
            var diseaseNode = _graph.CreateUriNode(new Uri(diseaseUri));

            _graph.Retract(new Triple(activeMaterialNode, curesNode, diseaseNode));

            SaveOntology();

            Console.WriteLine("Active material removed from the ontology and saved successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }
    public bool SetActiveMaterialAsCureForDisease(string activeMaterialUri, string diseaseUri)
    {
        try
        {
            var activeMaterialNode = _graph.CreateUriNode(new Uri(activeMaterialUri));
            var curesNode = _graph.CreateUriNode(new Uri("http://agroscan.com/ontology/cures"));
            var diseaseNode = _graph.CreateUriNode(new Uri(diseaseUri));

            _graph.Assert(new Triple(activeMaterialNode, curesNode, diseaseNode));

            SaveOntology();

            Console.WriteLine("Active material added to the ontology and saved successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }

    public bool AddAgroChemicalsToOntology(List<AgroChemicalDto> agroChemicals)
    {
        try
        {
            foreach (var agroChemical in agroChemicals)
            {
                AddAgroChemicalToOntology(agroChemical);
            }

            SaveOntology();
            
            Console.WriteLine("Agrochemicals added to the ontology and saved successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }
    private void AddAgroChemicalToOntology(AgroChemicalDto agroChemical)
    {
        var rdfTypeUri = _graph.CreateUriNode(new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#type"));
        var owlNamedIndividualUri = _graph.CreateUriNode(new Uri("http://www.w3.org/2002/07/owl#NamedIndividual"));
        var agroChemicalUri = _graph.CreateUriNode(new Uri(agroChemical.Uri));

        // Add triples for AgroChemical individual
        _graph.Assert(new Triple(agroChemicalUri, rdfTypeUri, owlNamedIndividualUri));
            _graph.Assert(new Triple(agroChemicalUri, rdfTypeUri, _graph.CreateUriNode(new Uri($"http://agroscan.com/ontology/AgroChemical"))));
        _graph.Assert(new Triple(agroChemicalUri, _graph.CreateUriNode(new Uri("http://agroscan.com/ontology/agroChemicalName")), _graph.CreateLiteralNode(agroChemical.Name)));
        _graph.Assert(new Triple(agroChemicalUri, _graph.CreateUriNode(new Uri("http://agroscan.com/ontology/belongsToAgroChemicalType")), _graph.CreateUriNode(new Uri("http://agroscan.com/ontology/"+ agroChemical.Type))));
        _graph.Assert(new Triple(agroChemicalUri, _graph.CreateUriNode(new Uri("http://agroscan.com/ontology/agroChemicalManufacturer")), _graph.CreateLiteralNode(agroChemical.Manufacturer)));
        _graph.Assert(new Triple(agroChemicalUri, _graph.CreateUriNode(new Uri("http://agroscan.com/ontology/agroChemicalRepresentative")), _graph.CreateLiteralNode(agroChemical.Representative)));

        // Add triples for each ActiveMaterial of AgroChemical
        foreach (var activeMaterialDto in agroChemical.ActiveMaterials)
        {
            var activeMaterialUri = _graph.CreateUriNode(new Uri($"http://agroscan.com/ontology/{activeMaterialDto.Name}"));
            _graph.Assert(new Triple(activeMaterialUri, rdfTypeUri, owlNamedIndividualUri));
            _graph.Assert(new Triple(activeMaterialUri, rdfTypeUri, _graph.CreateUriNode(new Uri($"http://agroscan.com/ontology/ActiveMaterial"))));
            _graph.Assert(new Triple(activeMaterialUri, _graph.CreateUriNode(new Uri("http://agroscan.com/ontology/activeMaterialName")), _graph.CreateLiteralNode(activeMaterialDto.Name)));

            var agroChemicalActiveMaterialUri = _graph.CreateUriNode(new Uri($"http://agroscan.com/ontology/{agroChemical.Name + "_" + activeMaterialDto.Name}"));

            _graph.Assert(new Triple(agroChemicalActiveMaterialUri, rdfTypeUri, owlNamedIndividualUri));
            _graph.Assert(new Triple(agroChemicalActiveMaterialUri, rdfTypeUri, _graph.CreateUriNode(new Uri($"http://agroscan.com/ontology/AgroChemicalActiveMaterial"))));
            _graph.Assert(new Triple(agroChemicalActiveMaterialUri, _graph.CreateUriNode(new Uri("http://agroscan.com/ontology/activeMaterialAmount")), _graph.CreateLiteralNode(activeMaterialDto.Amount)));
            _graph.Assert(new Triple(agroChemicalActiveMaterialUri, _graph.CreateUriNode(new Uri("http://agroscan.com/ontology/isActiveMaterial")), activeMaterialUri));

            _graph.Assert(new Triple(agroChemicalUri, _graph.CreateUriNode(new Uri("http://agroscan.com/ontology/contains")), agroChemicalActiveMaterialUri));
        }
    }
    private void SaveOntology()
    {
        using var fileStream = File.OpenWrite(_ontologyFilePath);
        using var streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
        var turtleWriter = new CompressingTurtleWriter();
        turtleWriter.Save(_graph, streamWriter);
    }
}
