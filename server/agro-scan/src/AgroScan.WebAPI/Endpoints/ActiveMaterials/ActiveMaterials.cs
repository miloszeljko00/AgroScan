using AgroScan.Application.Features.ActiveMaterials.Commands;
using AgroScan.Application.Features.ActiveMaterials.Queries;
using AgroScan.Application.Features.AgroChemicals.Commands;
using Microsoft.AspNetCore.Mvc;

namespace AgroScan.WebAPI.Endpoints.ActiveMaterials;
public static class ActiveMaterials
{
    public static void MapActiveMaterials(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/active-materials", GetAllActiveMaterials);
        app.MapPost("/api/v1/active-materials/cures", GetAllActiveMaterialsThatCuresDisease);
        app.MapPost("/api/v1/active-materials/set-cure", SetCure);
        app.MapPost("/api/v1/active-materials/revoke-cure", RevokeCure);
    }
    private static async Task<IResult> GetAllActiveMaterials(ISender sender)
    {
        var result = await sender.Send(new GetAllActiveMaterialsQueryRequest());
        return Results.Ok(result);
    }
    private static async Task<IResult> GetAllActiveMaterialsThatCuresDisease(ISender sender, [FromBody] GetAllActiveMaterialsThatCuresDiseaseDto request)
    {
        var result = await sender.Send(new GetAllActiveMaterialsThatCuresDiseaseQueryRequest() { DiseaseUri = request.diseaseUri });
        return Results.Ok(result);
    }
    private static async Task<IResult> SetCure(ISender sender, [FromBody] SetCureDto request)
    {
        var result = await sender.Send(new SetCureCommandRequest() 
        { 
            ActiveMaterialUri = request.ActiveMaterialUri,
            DiseaseUri = request.DiseaseUri 
        });
        return Results.Ok(result);
    }
    private static async Task<IResult> RevokeCure(ISender sender, [FromBody] RevokeCureDto request)
    {
        var result = await sender.Send(new RevokeCureCommandRequest()
        {
            ActiveMaterialUri = request.ActiveMaterialUri,
            DiseaseUri = request.DiseaseUri
        });
        return Results.Ok(result);
    }
}
