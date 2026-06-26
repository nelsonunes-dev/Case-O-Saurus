using CaseOSaurus.Application;
using CaseOSaurus.Infrastructure;
using CaseOSaurus.Infrastructure.Persistence;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((context, cfg) =>
    cfg.ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day));

// Add services
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration, false);

// CORS (for Blazor)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
    {
        policy.WithOrigins("http://localhost:5198") // your Blazor URL
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// FastEndpoints and OpenAPI
builder.Services.AddFastEndpoints();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocument();

var app = builder.Build();

// Apply migrations automatically (optional)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}

// CORS
app.UseCors("AllowBlazor");

// FastEndpoints pipeline
app.UseFastEndpoints(c =>
{
    c.Errors.ResponseBuilder = (failures, ctx, statusCode) =>
    {
        return new
        {
            status = statusCode,
            errors = failures.Select(f => new { f.PropertyName, f.ErrorMessage })
        };
    };
});

// Swagger UI
app.UseOpenApi();
app.UseSwaggerUi(s => s.ConfigureDefaults());

app.Run();

public partial class Program { }
