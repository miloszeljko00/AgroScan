using AgroScan.Application.Features.AgroChemicals.Commands;
using AgroScan.Application.Features.AgroChemicals.Queries;
using Microsoft.AspNetCore.Mvc;

namespace AgroScan.WebAPI.Endpoints.AgroChemicals;
public static class AgroChemicals
{
    public static void MapAgroChemicals(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/agrochemicals/get-recommendation", GetRecommendationForDisease);
        app.MapPost("/api/v1/agrochemicals/parse-from-excel", ParseFromExcel).DisableAntiforgery();
    }
    private static async Task<IResult> GetRecommendationForDisease(ISender sender, [FromBody] GetRecommendationForDiseaseDto request)
    {
        var result = await sender.Send(new GetRecommendationForDiseaseQuery() { DiseaseUri = request.DiseaseUri });
        return Results.Ok(result);
    }
    private static async Task<IResult> ParseFromExcel(ISender sender, [FromForm] IFormFile excelFile)
    {
        if (excelFile != null)
        {
            try
            {
                using var memoryStream = new MemoryStream();
                await excelFile.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                var result = await sender.Send(new ParseFromExcelCommand() { ExcelStream = memoryStream });
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest($"Error processing Excel file: {ex.Message}");
            }
        }
        else
        {
            return Results.BadRequest("No Excel file found in the request.");
        }
    }

}
