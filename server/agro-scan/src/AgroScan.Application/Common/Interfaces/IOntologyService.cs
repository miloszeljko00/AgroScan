using AgroScan.Application.Features.AgroChemicals.Commands;
using VDS.RDF;
using VDS.RDF.Query;

namespace AgroScan.Application.Common.Interfaces;
public interface IOntologyService
{
    public bool AddAgroChemicalsToOntology(List<AgroChemicalDto> agroChemicals);
    public bool RevokeActiveMaterialAsCureForDisease(string activeMaterialUri, string DiseaseNameUri);
    public bool SetActiveMaterialAsCureForDisease(string activeMaterialUri, string DiseaseNameUri);
    SparqlResultSet? ExecuteSparqlQuery(string query);
    string? GetValue(INode node);
}
