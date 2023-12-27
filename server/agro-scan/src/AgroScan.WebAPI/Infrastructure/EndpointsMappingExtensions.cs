using AgroScan.WebAPI.Endpoints.AgroChemicals;

namespace AgroScan.WebAPI.Infrastructure;

public static class EndpointsMappingExtensions
{
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapAgroChemicals();
        return app;
    }
}
