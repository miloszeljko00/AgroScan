using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroScan.Application.Features.AgroChemicals;
public class AgroChemicalDto
{
    public string Uri { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public List<ActiveMaterialDto> ActiveMaterials { get; set; } = [];
    public string Manufacturer { get; set; } = string.Empty;
    public string Representative { get; set; } = string.Empty;
}
