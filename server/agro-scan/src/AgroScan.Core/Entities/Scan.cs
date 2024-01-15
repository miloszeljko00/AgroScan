namespace AgroScan.Core.Entities;
public class Scan
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string ImageBase64 { get; set; } = string.Empty;
    public string PlantUri { get; set; } = string.Empty;
    public string PlantName { get; set; } = string.Empty;
    public string? DiseaseUri { get; set; }
    public string? DiseaseName { get; set; }
}
