using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroScan.Application.Common.Exceptions;
public class NotFoundException(string name, object key) : Exception($"Entity \"{name}\" ({key}) was not found.")
{
}
