using AgroScan.Application.Common.Interfaces;
using AgroScan.Application.Common.Models;
using OfficeOpenXml;
using System.IO;

namespace AgroScan.Application.Features.AgroChemicals.Commands;
public record ParseFromExcelCommand : IRequest<IReadOnlyCollection<AgroChemicalDto>>
{
    public MemoryStream? ExcelStream { get; set; }
}

public class ParseFromExcelCommandHandler : IRequestHandler<ParseFromExcelCommand, IReadOnlyCollection<AgroChemicalDto>>
{
    public async Task<IReadOnlyCollection<AgroChemicalDto>> Handle(ParseFromExcelCommand request, CancellationToken cancellationToken)
    {
        var agroChemicals = new List<AgroChemicalDto>();

        using var package = new ExcelPackage(request.ExcelStream);
        var herbicides = ParseAgroChemicalsWithType("Herbicide", package.Workbook.Worksheets[0]);
        var growthRegulators = ParseAgroChemicalsWithType("GrowthRegulator", package.Workbook.Worksheets[1]);
        var fungicides = ParseAgroChemicalsWithType("Fungicide", package.Workbook.Worksheets[2]);
        var fungicidesForSeedTreatment = ParseAgroChemicalsWithType("FungicideForSeedTreatment", package.Workbook.Worksheets[3]);
        var insecticides = ParseAgroChemicalsWithType("Insecticide", package.Workbook.Worksheets[4]);
        var insecticidesForSeedTreatment = ParseAgroChemicalsWithType("InsecticideForSeedTreatment", package.Workbook.Worksheets[5]);
        var acaricides = ParseAgroChemicalsWithType("Acaricide", package.Workbook.Worksheets[6]);
        var nematocides = ParseAgroChemicalsWithType("Nematocide", package.Workbook.Worksheets[7]);
        var limacids = ParseAgroChemicalsWithType("Limacid", package.Workbook.Worksheets[8]);
        var rodenticides = ParseAgroChemicalsWithType("Rodenticide", package.Workbook.Worksheets[9]);
        var repellents = ParseAgroChemicalsWithType("Repellent", package.Workbook.Worksheets[10]);
        var disinfectants = ParseAgroChemicalsWithType("Disinfectant", package.Workbook.Worksheets[11]);
        var bioGrowthRegulators = ParseAgroChemicalsWithType("BioGrowthRegulator", package.Workbook.Worksheets[12]);
        var bioFungicidesMicrobiological = ParseAgroChemicalsWithType("BioFungicideMicrobiological", package.Workbook.Worksheets[13]);
        var bioFungicidesBiochemical = ParseAgroChemicalsWithType("BioFungicideBiochemical", package.Workbook.Worksheets[14]);
        var bioInsecticidesMicrobiological = ParseAgroChemicalsWithType("BioInsecticideMicrobiological", package.Workbook.Worksheets[15]);
        var bioAcaricidesMicrobiological = ParseAgroChemicalsWithType("BioAcaricideMicrobiological", package.Workbook.Worksheets[16]);
        var adjuvents = ParseAgroChemicalsWithType("Adjuvent", package.Workbook.Worksheets[17]);

        agroChemicals.AddRange(herbicides);
        agroChemicals.AddRange(growthRegulators);
        agroChemicals.AddRange(fungicides);
        agroChemicals.AddRange(fungicidesForSeedTreatment);
        agroChemicals.AddRange(insecticides);
        agroChemicals.AddRange(insecticidesForSeedTreatment);
        agroChemicals.AddRange(acaricides);
        agroChemicals.AddRange(nematocides);
        agroChemicals.AddRange(limacids);
        agroChemicals.AddRange(rodenticides);
        agroChemicals.AddRange(repellents);
        agroChemicals.AddRange(disinfectants);
        agroChemicals.AddRange(bioGrowthRegulators);
        agroChemicals.AddRange(bioFungicidesMicrobiological);
        agroChemicals.AddRange(bioFungicidesBiochemical);
        agroChemicals.AddRange(bioInsecticidesMicrobiological);
        agroChemicals.AddRange(bioAcaricidesMicrobiological);
        agroChemicals.AddRange(adjuvents);

        return agroChemicals;
    }

    private List<AgroChemicalDto> ParseAgroChemicalsWithType(string type, ExcelWorksheet worksheet)
    {
        var agroChemicals = new List<AgroChemicalDto>();
        for (int row = 2; row <= worksheet.Dimension.Rows; row++)
        {
            
            var name = worksheet.Cells[row, 1].Value.ToString() ?? "";
            var activeMaterialNames = (worksheet.Cells[row, 2].Value.ToString() ?? "").Split("+");
            var activeMaterialAmounts = (worksheet.Cells[row, 3].Value.ToString() ?? "").Split("+");
            var manufacturer = worksheet.Cells[row, 4].Value.ToString() ?? "";
            var representative = worksheet.Cells[row, 5].Value.ToString() ?? "";

            if (activeMaterialNames.Length != activeMaterialAmounts.Length)
                throw new Exception($"Type[{type}]-Row[{row}]: Active materials and active material amounts count mismatch for agrochemical: {name}.");
            
            var activeMaterials = new List<ActiveMaterialDto>();
            
            for (int i = 0; i < activeMaterialNames.Length; i++)
            {
                activeMaterials.Add(new ActiveMaterialDto()
                {
                    Amount = activeMaterialAmounts[i],
                    Name = activeMaterialNames[i] 
                });  
            }

            agroChemicals.Add(new AgroChemicalDto()
            {
                Name = name,
                Type = type,
                ActiveMaterials = activeMaterials,
                Manufacturer = manufacturer,
                Representative = representative
            });

        }
        return agroChemicals;
    }
}
