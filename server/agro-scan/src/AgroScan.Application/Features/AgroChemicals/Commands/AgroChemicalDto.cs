using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroScan.Application.Features.AgroChemicals.Commands;
public class AgroChemicalDto
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public List<ActiveMaterialDto> ActiveMaterials { get; set; } = [];
    public string Manufacturer { get; set; } = string.Empty;
    public string Representative { get; set; } = string.Empty;
}
