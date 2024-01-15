using AgroScan.Application.Common.Interfaces;
using AgroScan.Application.Features.Scans.Queries;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AgroScan.WebAPI.Endpoints.Scans;
public static class Scans
{
    public static void MapScans(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/scans", GetAllScans).RequireAuthorization();
        app.MapPost("/api/v1/scans/create", CreateScan).DisableAntiforgery().RequireAuthorization();
    }

    private static async Task<IResult> GetAllScans(ISender sender, IUser user)
    {
        var result = await sender.Send(new GetAllScansByUserIdQueryRequest() 
        { 
            UserId = user.Id ?? string.Empty
        });
        return Results.Ok(result);
    }

    private static async Task<IResult> CreateScan(ISender sender, IUser user, [FromForm] IFormFile image)
    {
        if (image == null || image.Length == 0)
        {
            return Results.BadRequest("Image file is required.");
        }
        var result = await sender.Send(new CreateScanCommandRequest()
        {
            UserId = user.Id ?? string.Empty,
            ImageBase64 = ConvertFileToBase64(image),
        });
        return Results.Ok(result);
    }

    private static string ConvertFileToBase64(IFormFile file)
    {
        using var memoryStream = new MemoryStream();
        file.CopyTo(memoryStream);
        return Convert.ToBase64String(memoryStream.ToArray());
    }
}
