using VDS.RDF;
using VDS.RDF.Query;

namespace AgroScan.Application.Common.Interfaces;
public interface IOntologyService
{
    SparqlResultSet? ExecuteSparqlQuery(string query);
    string? GetValue(INode node);
}
