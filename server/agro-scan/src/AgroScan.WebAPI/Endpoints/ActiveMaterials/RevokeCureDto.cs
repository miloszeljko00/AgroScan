namespace AgroScan.WebAPI.Endpoints.ActiveMaterials;

public class RevokeCureDto
{
    public string ActiveMaterialUri { get; set; } = string.Empty;
    public string DiseaseUri { get; set; } = string.Empty;
}
