using AgroScan.WebAPI.Endpoints.ActiveMaterials;
using AgroScan.WebAPI.Endpoints.AgroChemicals;
using AgroScan.WebAPI.Endpoints.Diseases;
using AgroScan.WebAPI.Endpoints.Plants;

namespace AgroScan.WebAPI.Infrastructure;

public static class EndpointsMappingExtensions
{
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapAgroChemicals();
        app.MapDiseases();
        app.MapPlants();
        app.MapActiveMaterials();

        return app;
    }
}
