using CaseOSaurus.Application;
using CaseOSaurus.Infrastructure;
using CaseOSaurus.Infrastructure.Persistence;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog(static (context, cfg) =>
    cfg.ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day));

// Add services
builder.Services.AddApplication();
// Allow tests to skip DB provider registration by passing a flag when running IntegrationTests.
var skipDb = builder.Environment.IsEnvironment("IntegrationTests");
builder.Services.AddInfrastructure(builder.Configuration, skipDb);

// FastEndpoints
builder.Services.AddFastEndpoints();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocument();

var app = builder.Build();

// Apply migrations automatically (optional) - skip when running IntegrationTests environment (tests may use InMemory provider)
if (!app.Environment.IsEnvironment("IntegrationTests"))
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}

// FastEndpoints
app.UseFastEndpoints(c =>
{
    c.Errors.ResponseBuilder = (failures, ctx, statusCode) =>
    {
        // Return validation errors in a standard format
        return new
        {
            status = statusCode,
            errors = failures.Select(f => new { f.PropertyName, f.ErrorMessage })
        };
    };
});

app.UseOpenApi();
app.UseSwaggerUi(s => s.ConfigureDefaults());

app.Run();

public partial class Program { }