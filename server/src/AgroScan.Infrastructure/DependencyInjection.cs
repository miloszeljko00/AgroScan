using AgroScan.Application.Common.Interfaces;
using AgroScan.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF;
using AgroScan.Infrastructure.Ontology;

namespace Microsoft.Extensions.DependencyInjection;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if(string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("DefaultConnection is null or empty.");
        }

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
            });
        });
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<ApplicationDbContextInitialiser>();
        services.AddSingleton(TimeProvider.System);


        // Add RDF services as needed
        services.AddSingleton<IGraph>(provider =>
        {
            IGraph graph = new Graph();
            string ontologyFilePath = System.IO.Path.Combine(AppContext.BaseDirectory, "Ontology", "agroscan.ttl");
            FileLoader.Load(graph, ontologyFilePath, new TurtleParser());
            return graph;
        });
        services.AddSingleton<TripleStore>(provider =>
        {
            var tripleStore = new TripleStore();
            var graph = provider.GetRequiredService<IGraph>();
            tripleStore.Add(graph);
            return tripleStore;
        });
        services.AddTransient<LeviathanQueryProcessor>(provider =>
        {
            var tripleStore = provider.GetRequiredService<TripleStore>();
            return new LeviathanQueryProcessor(tripleStore);
        });
        services.AddSingleton<ISparqlQueryProcessor, LeviathanQueryProcessor>(provider =>
        {
            var leviathanQueryProcessor = provider.GetRequiredService<LeviathanQueryProcessor>();
            return leviathanQueryProcessor;
        });
        services.AddScoped<IOntologyService, OntologyService>();
        return services;
    }
}
