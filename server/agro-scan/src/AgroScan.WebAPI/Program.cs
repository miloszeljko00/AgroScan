using AgroScan.Infrastructure.Data;
using AgroScan.WebAPI.Infrastructure;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebServices();

var app = builder.Build();
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "{documentName}/swagger.json";
    });
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("v1/swagger.json", "AgroScan API");
        c.RoutePrefix = "";
    });
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHealthChecks("/health");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapFallbackToFile("index.html");

app.UseExceptionHandler(options => { });
app.MapEndpoints();

app.Run();

public partial class Program { }
