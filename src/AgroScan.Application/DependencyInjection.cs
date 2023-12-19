using AgroScan.Application.Common.Behaviours;
using System.Reflection;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;

namespace Microsoft.Extensions.DependencyInjection;
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
        });

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

        return services;
    }
}
