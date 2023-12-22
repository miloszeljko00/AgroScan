using AgroScan.Application.Common.Interfaces;
using AgroScan.Application.Common.Models;

namespace AgroScan.Application.Features.AgroChemicals.Commands;
public record ParseFromExcelCommand : IRequest<bool>
{
}

public class ParseFromExcelCommandHandler : IRequestHandler<ParseFromExcelCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public ParseFromExcelCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(ParseFromExcelCommand request, CancellationToken cancellationToken)
    {
        return true;
    }
}
