using CaseOSaurus.Domain.Entities;
using CaseOSaurus.Domain.Enums;
using CaseOSaurus.Infrastructure.Persistence;
using FastEndpoints;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Testcontainers.MsSql;

namespace CaseOSaurus.IntegrationTests.Endpoints;

public class AssignCaseEndpointTests : IAsyncLifetime
{
    // Response DTO shape for assignment
    private record AssignResponse(
        Guid Id,
        string Title,
        string? Description,
        string Priority,
        string Type,
        string Requestor,
        string? AssignedTo,
        string Status,
        DateTime CreatedAt,
        string CreatedBy
    );

    private readonly MsSqlContainer _sqlContainer;
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    public AssignCaseEndpointTests()
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
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(_sqlContainer.GetConnectionString()));

                services.AddEndpointsApiExplorer();
                services.AddFastEndpoints();
            });
        });

        _client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _sqlContainer.DisposeAsync();
        await _factory.DisposeAsync();
    }

    [Fact]
    public async Task PatchAssignWithValidRequestShouldReturnOk()
    {
        // Arrange - create a case first
        var caseEntity = new UserCase(
            "Integration Test",
            "For assignment",
            Priority.High,
            CaseType.Incident,
            "test@example.com",
            "creator"
        );
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Cases.Add(caseEntity);
        await dbContext.SaveChangesAsync();

        var request = new { AssignedTo = "assignee-456" };

        // Act
        var response = await _client.PatchAsJsonAsync($"/api/cases/{caseEntity.Id}/assign", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<AssignResponse>();
        result.Should().NotBeNull();
        result!.AssignedTo.Should().Be("assignee-456");
        result.Id.Should().Be(caseEntity.Id);
    }

    [Fact]
    public async Task PatchAssignWithInvalidCaseIdShouldReturnNotFound()
    {
        // Arrange
        var request = new { AssignedTo = "user-123" };

        // Act
        var response = await _client.PatchAsJsonAsync($"/api/cases/{Guid.NewGuid()}/assign", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PatchAssignWithEmptyAssignedToShouldReturnBadRequest()
    {
        // Arrange
        var caseEntity = new UserCase(
            "Test",
            "Desc",
            Priority.Low,
            CaseType.ServiceRequest,
            "test@example.com",
            "creator"
        );
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Cases.Add(caseEntity);
        await dbContext.SaveChangesAsync();

        var request = new { AssignedTo = "" };

        // Act
        var response = await _client.PatchAsJsonAsync($"/api/cases/{caseEntity.Id}/assign", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var errorContent = await response.Content.ReadAsStringAsync();
        errorContent.Should().Contain("AssignedTo is required");
    }
}
