using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF;
using AgroScan.Application.Common.Interfaces;

namespace AgroScan.Infrastructure.Ontology;
internal class OntologyService(ISparqlQueryProcessor sparqlQueryProcessor) : IOntologyService
{
    private ISparqlQueryProcessor _sparqlQueryProcessor = sparqlQueryProcessor;

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
}
