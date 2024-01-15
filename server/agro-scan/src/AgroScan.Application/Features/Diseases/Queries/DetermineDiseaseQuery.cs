using AgroScan.Core.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Net.Http.Json;
namespace AgroScan.Application.Features.Diseases.Queries;

public class DetermineDiseaseQueryRequest : IRequest<Disease?>
{
    public string ImageBase64 { get; set; } = string.Empty;
    public string PlantUri { get; set; } = string.Empty;
}

public class DetermineDiseaseQueryHandler(IConfiguration configuration) : IRequestHandler<DetermineDiseaseQueryRequest, Disease?>
{
    private readonly IConfiguration _configuration = configuration;
    public async Task<Disease?> Handle(DetermineDiseaseQueryRequest request, CancellationToken cancellationToken)
    {
        return request.PlantUri switch
        {
            "http://agroscan.com/ontology/tomato" => await DetermineTomatoDisease(request.ImageBase64),
            _ => throw new NotImplementedException(),
        };
    }

    private async Task<Disease?> DetermineTomatoDisease(string imageBase64)
    {
        string apiUrl = _configuration["TomatoDiseasePredictionAPI"] ?? "";

        var requestData = new { image = imageBase64 };

        using var client = new HttpClient();
        var response = await client.PostAsJsonAsync(apiUrl, requestData);
        response.EnsureSuccessStatusCode();

        var resultJson = await response.Content.ReadAsStringAsync();
        dynamic? result = JsonConvert.DeserializeObject(resultJson);

        string predictedLabel = result?.label ?? "";

        Console.WriteLine($"Predicted Label: {predictedLabel}");

        return GetDiseaseFromLabel(predictedLabel);
    }

    private static Disease? GetDiseaseFromLabel(string label)
    {
        var disease = new Disease();
        switch (label)
        {
            case "Tomato___Bacterial_spot":
                disease.Name = "Bacterial Spot";
                disease.Uri = "http://agroscan.com/ontology/bacterial_spot";
                break;
            case "Tomato___Early_blight":
                disease.Name = "Early blight";
                disease.Uri = "http://agroscan.com/ontology/early_blight";
                break;
            case "Tomato___Late_blight":
                disease.Name = "Late blight";
                disease.Uri = "http://agroscan.com/ontology/late_blight";
                break;
            case "Tomato___Leaf_Mold":
                disease.Name = "Leaf mold";
                disease.Uri = "http://agroscan.com/ontology/leaf_mold";
                break;
            case "Tomato___Septoria_leaf_spot":
                disease.Name = "Septoria leaf spot";
                disease.Uri = "http://agroscan.com/ontology/septoria_leaf_spot";
                break;
            case "Tomato___Spider_mites Two-spotted_spider_mite":
                disease.Name = "Spider mites (Two-Spotted spider mite)";
                disease.Uri = "http://agroscan.com/ontology/spider_mites_two-spotted_spider_mite";
                break;
            case "Tomato___Target_Spot":
                disease.Name = "Target spot";
                disease.Uri = "http://agroscan.com/ontology/target_spot";
                break;
            case "Tomato___Tomato_Yellow_Leaf_Curl_Virus":
                disease.Name = "Yellow leaf curl virus (TYLCV)";
                disease.Uri = "http://agroscan.com/ontology/yellow_leaf_curl_virus";
                break;
            case "Tomato___Tomato_mosaic_virus":
                disease.Name = "Mosaic virus (ToMV)";
                disease.Uri = "http://agroscan.com/ontology/mosaic_virus";
                break;
            case "Tomato___healthy":
            default:
                disease = null;
                break;
        }
        return disease;
    }
}