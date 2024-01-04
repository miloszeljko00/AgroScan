using AgroScan.Application.Features.Plants.Queries;

namespace AgroScan.WebAPI.Endpoints.Plants;
public static class Plants
{
    public static void MapPlants(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/plants", GetAllPlants);
    }
    private static async Task<IResult> GetAllPlants(ISender sender)
    {
        var result = await sender.Send(new GetAllPlantsQueryRequest());
        return Results.Ok(result);
    }

}
