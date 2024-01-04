using AgroScan.Application.Features.ActiveMaterials.Queries;

namespace AgroScan.WebAPI.Endpoints.ActiveMaterials;
public static class ActiveMaterials
{
    public static void MapActiveMaterials(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/active-materials", GetAllActiveMaterials);
        app.MapGet("/api/v1/active-materials-by-disease-name", GetAllActiveMaterialsByDiseaseName);
    }
    private static async Task<IResult> GetAllActiveMaterials(ISender sender)
    {
        var result = await sender.Send(new GetAllActiveMaterialsQueryRequest());
        return Results.Ok(result);
    }
    private static async Task<IResult> GetAllActiveMaterialsByDiseaseName(ISender sender, string diseaseName)
    {
        var result = await sender.Send(new GetAllActiveMaterialsByDiseaseNameQueryRequest() { DiseaseName = diseaseName });
        return Results.Ok(result);
    }
}
