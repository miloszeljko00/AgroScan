using AgroScan.Application.Features.AgroChemicals.Commands;

namespace AgroScan.WebAPI.Endpoints.AgroChemicals;
public static class AgroChemicals
{
    public static void MapAgroChemicals(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/agrochemicals/get-all", GetAll);
        app.MapGet("/api/v1/agrochemicals/parse-from-excel", ParseFromExcel);
    }

    private static async Task<IResult> GetAll(ISender sender)
    {
        var result = await sender.Send(new ParseFromExcelCommand());
        return Results.Ok(result);
    }
    private static async Task<IResult> ParseFromExcel(ISender sender)
    {
        var result = await sender.Send(new ParseFromExcelCommand());
        return Results.Ok(result);
    }
}
