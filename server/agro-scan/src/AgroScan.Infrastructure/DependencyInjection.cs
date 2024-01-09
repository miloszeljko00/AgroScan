using AgroScan.Application.Common.Interfaces;
using AgroScan.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF;
using AgroScan.Infrastructure.Ontology;
using AgroScan.Infrastructure.Data.Interceptors;
using Microsoft.EntityFrameworkCore.Diagnostics;
using AgroScan.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System.Data;
using AgroScan.Core.Constants;

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
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
            });
        });
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<ApplicationDbContextInitialiser>();

        services.AddAuthentication()
            .AddBearerToken(IdentityConstants.BearerScheme);

        services.AddAuthorizationBuilder();

        services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddApiEndpoints();

        services.AddSingleton(TimeProvider.System);
        services.AddTransient<IIdentityService, IdentityService>();

        // Policy-based authorization example
        //services.AddAuthorization(options =>
        //    options.AddPolicy(Policies.CanPurge, policy => policy.RequireRole(Roles.Administrator)));

        // Add RDF services as needed
        services.AddSingleton<IGraph>(provider =>
        {
            IGraph graph = new Graph();
            string ontologyFilePath = configuration["OntologyFilePath"] ?? "";
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
