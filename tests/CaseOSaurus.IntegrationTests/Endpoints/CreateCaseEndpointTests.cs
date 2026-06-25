using CaseOSaurus.API.Endpoints.Cases;
using CaseOSaurus.Domain.Enums;
using CaseOSaurus.Infrastructure.Persistence;
using FastEndpoints;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using System.Net;
using System.Net.Http.Json;
using Testcontainers.MsSql;

namespace CaseOSaurus.IntegrationTests.Endpoints;

public class CreateCaseEndpointTests : IAsyncLifetime
{
    private readonly MsSqlContainer _sqlContainer;
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    public CreateCaseEndpointTests()
    {
        _sqlContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/azure-sql-edge:latest")
            .WithPassword("YourStrong!Passw0rd")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _sqlContainer.StartAsync();

        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            // Run app in a test environment so startup can adapt (skip relational migrations)
            builder.UseEnvironment("IntegrationTests");

            builder.ConfigureServices(services =>
            {
                // Remove any existing ApplicationDbContext / DbContextOptions registrations (SQL Server) so we can use InMemory for tests
                var descriptors = services.Where(d =>
                    d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>) ||
                    d.ServiceType == typeof(ApplicationDbContext) ||
                    (d.ImplementationType != null && d.ImplementationType == typeof(ApplicationDbContext))
                ).ToList();

                foreach (var d in descriptors)
                    services.Remove(d);

                // For reliable, fast test runs use an in-memory database so we don't depend on container DDL
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase("IntegrationTestsDb"));

                // Make IApplicationDbContext resolve to the in-memory ApplicationDbContext for handlers/services
                services.AddScoped<CaseOSaurus.Application.Common.Interfaces.IApplicationDbContext>(provider =>
                    provider.GetRequiredService<ApplicationDbContext>());

                // 🔧 FIX: Ensure API Explorer is registered for Swagger/OpenAPI
                services.AddEndpointsApiExplorer();

                // 🔧 Also ensure FastEndpoints is correctly registered (though it's already in Program)
                // This line is optional because the real Program already does it,
                // but we can add it for safety; it won't duplicate.
                services.AddFastEndpoints();
            });
        });

        _client = _factory.CreateClient();

        // Ensure database schema exists for tests (no migrations in repo)
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _sqlContainer.DisposeAsync();
        await _factory.DisposeAsync();
    }

    [Fact]
    public async Task PostValidRequestShouldReturnCreatedCase()
    {
        // Arrange
        var request = new
        {
            Title = "Integration Test Case",
            Description = "Created via integration test",
            Priority = Priority.High.ToString(),
            Type = CaseType.Incident.ToString(),
            Requestor = "test@example.com",
            AssignedTo = "user-123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/cases", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<CreateCaseResponse>();
        result.Should().NotBeNull();
        result!.Id.Should().NotBeEmpty();
        result.Title.Should().Be("Integration Test Case");
        result.Status.Should().Be(CaseStatus.Open.ToString());
    }

    [Fact]
    public async Task PostInvalidRequestShouldReturnBadRequest()
    {
        // Arrange - missing Title and Requestor
        var request = new
        {
            Description = "Invalid case",
            Priority = Priority.Medium.ToString(),
            Type = CaseType.ServiceRequest.ToString()
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/cases", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var errorContent = await response.Content.ReadAsStringAsync();
        errorContent.Should().Contain("Title is required");
        errorContent.Should().Contain("Requestor is required");
    }
}
