using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace AgroScan.Application.Common.Behaviours;

internal class LoggingBehaviour<TRequest>(ILogger<TRequest> logger) : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    private readonly ILogger _logger = logger;

    public Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        _logger.LogInformation("AgroScan Request: {Name} - {@Request}",
            requestName, request);

        return Task.CompletedTask;
    }
}