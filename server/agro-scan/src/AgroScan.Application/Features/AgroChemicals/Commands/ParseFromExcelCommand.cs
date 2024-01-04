using AgroScan.Application.Common.Interfaces;
using AgroScan.Application.Common.Models;
using AgroScan.Core.Entities;
using OfficeOpenXml;
using System.IO;

namespace AgroScan.Application.Features.AgroChemicals.Commands;
public record ParseFromExcelCommand : IRequest<IReadOnlyCollection<AgroChemicalDto>>
{
    public MemoryStream? ExcelStream { get; set; }
}

public class ParseFromExcelCommandHandler(IOntologyService ontologyService) : IRequestHandler<ParseFromExcelCommand, IReadOnlyCollection<AgroChemicalDto>>
{
    private readonly IOntologyService ontologyService = ontologyService;

    public async Task<IReadOnlyCollection<AgroChemicalDto>> Handle(ParseFromExcelCommand request, CancellationToken cancellationToken)
    {
        var agroChemicals = new List<AgroChemicalDto>();

        using var package = new ExcelPackage(request.ExcelStream);
        var herbicides = ParseAgroChemicalsWithType("herbicide", package.Workbook.Worksheets[0]);
        var growthRegulators = ParseAgroChemicalsWithType("growth_regulator", package.Workbook.Worksheets[1]);
        var fungicides = ParseAgroChemicalsWithType("fungicide", package.Workbook.Worksheets[2]);
        var fungicidesForSeedTreatment = ParseAgroChemicalsWithType("fungicide_for_seed_treatment", package.Workbook.Worksheets[3]);
        var insecticides = ParseAgroChemicalsWithType("insecticide", package.Workbook.Worksheets[4]);
        var insecticidesForSeedTreatment = ParseAgroChemicalsWithType("insecticide_for_seed_treatment", package.Workbook.Worksheets[5]);
        var acaricides = ParseAgroChemicalsWithType("acaricide", package.Workbook.Worksheets[6]);
        var nematocides = ParseAgroChemicalsWithType("nematocide", package.Workbook.Worksheets[7]);
        var limacids = ParseAgroChemicalsWithType("limacid", package.Workbook.Worksheets[8]);
        var rodenticides = ParseAgroChemicalsWithType("rodenticide", package.Workbook.Worksheets[9]);
        var repellents = ParseAgroChemicalsWithType("repellent", package.Workbook.Worksheets[10]);
        var disinfectants = ParseAgroChemicalsWithType("disinfectant", package.Workbook.Worksheets[11]);
        var bioGrowthRegulators = ParseAgroChemicalsWithType("biogrowth_regulator", package.Workbook.Worksheets[12]);
        var bioFungicidesMicrobiological = ParseAgroChemicalsWithType("biofungicide_microbiological", package.Workbook.Worksheets[13]);
        var bioFungicidesBiochemical = ParseAgroChemicalsWithType("biofungicide_biochemical", package.Workbook.Worksheets[14]);
        var bioInsecticidesMicrobiological = ParseAgroChemicalsWithType("bioinsecticide_microbiological", package.Workbook.Worksheets[15]);
        var bioAcaricidesMicrobiological = ParseAgroChemicalsWithType("bioacaricide_microbiological", package.Workbook.Worksheets[16]);
        var adjuvents = ParseAgroChemicalsWithType("adjuvent", package.Workbook.Worksheets[17]);

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
            
            var name = worksheet.Cells[row, 1].Value.ToString()?.Trim().Replace("\n", "").Replace("\r", "") ?? "";
            var activeMaterialNames = (worksheet.Cells[row, 2].Value.ToString()?.Trim().Replace("\n", "").Replace("\r", "") ?? "").Split("+");
            var activeMaterialAmounts = (worksheet.Cells[row, 3].Value.ToString()?.Trim().Replace("\n", "").Replace("\r", "") ?? "").Split("+");
            var manufacturer = worksheet.Cells[row, 4].Value.ToString()?.Trim().Replace("\n", "").Replace("\r", "") ?? "";
            var representative = worksheet.Cells[row, 5].Value.ToString()?.Trim().Replace("\n", "").Replace("\r", "") ?? "";

            if (activeMaterialNames.Length != activeMaterialAmounts.Length)
                throw new Exception($"Type[{type}]-Row[{row}]: Active materials and active material amounts count mismatch for agrochemical: {name}.");
            
            var activeMaterials = new List<ActiveMaterialDto>();
            
            for (int i = 0; i < activeMaterialNames.Length; i++)
            {
                activeMaterials.Add(new ActiveMaterialDto()
                {
                    Amount = activeMaterialAmounts[i].Trim().Replace("\n", "").Replace("\r", ""),
                    Name = activeMaterialNames[i].Trim().Replace("\n", "").Replace("\r", "")
                });  
            }

            agroChemicals.Add(new AgroChemicalDto()
            {
                Uri = new Uri($"http://agroscan.com/ontology/{name}").ToString(),
                Name = name,
                Type = type,
                ActiveMaterials = activeMaterials,
                Manufacturer = manufacturer,
                Representative = representative
            });

        }
        if (ontologyService.AddAgroChemicalsToOntology(agroChemicals) == false)
        {
            throw new Exception($"Type[{type}]: AgroChemicalDto list was not added to the ontology.");
        }
        return agroChemicals;
    }
}
