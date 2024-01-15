using AgroScan.WebAPI.Endpoints.ActiveMaterials;
using AgroScan.WebAPI.Endpoints.AgroChemicals;
using AgroScan.WebAPI.Endpoints.Diseases;
using AgroScan.WebAPI.Endpoints.Plants;
using AgroScan.WebAPI.Endpoints.Scans;
using AgroScan.WebAPI.Endpoints.Users;

namespace AgroScan.WebAPI.Infrastructure;

public static class EndpointsMappingExtensions
{
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapUsers();
        app.MapAgroChemicals();
        app.MapDiseases();
        app.MapPlants();
        app.MapActiveMaterials();
        app.MapScans();

        return app;
    }
}
