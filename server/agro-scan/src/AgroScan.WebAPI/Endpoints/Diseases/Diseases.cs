using AgroScan.Application.Features.Diseases.Queries;

namespace AgroScan.WebAPI.Endpoints.Diseases;
public static class Diseases
{
    public static void MapDiseases(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/diseases/infects", GetAllDiseasesByPlantName);
    }
    private static async Task<IResult> GetAllDiseasesByPlantName(ISender sender, string plantName)
    {
        var result = await sender.Send(new GetAllDiseasesByPlantNameQueryRequest() { PlantName = plantName });
        return Results.Ok(result);
    }

}
