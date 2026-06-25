using AutoMapper;
using CaseOSaurus.Application.Commands.CreateCase;
using CaseOSaurus.Application.Common.Services;
using CaseOSaurus.Domain.Enums;
using CaseOSaurus.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace CaseOSaurus.UnitTests.Commands.CreateCase;

public class CreateCaseCommandHandlerTests
{
    [Fact]
    public async Task HandleValidCommandShouldCreateAndReturnCaseResponse()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);

        var mapper = new MapperConfiguration((cfg => cfg.AddProfile<Application.Common.Mappings.MappingProfile>()), NullLoggerFactory.Instance).CreateMapper();

        var userContextMock = new Mock<IUserContext>();
        userContextMock.Setup(u => u.UserId).Returns("test-user-123");

        var handler = new CreateCaseCommandHandler(context, mapper, userContextMock.Object);

        var command = new CreateCaseCommand(
            "Test Case",
            "This is a test description",
            Priority.Medium,
            CaseType.Incident,
            "requester@example.com"
        );

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.Title.Should().Be("Test Case");
        result.Status.Should().Be(CaseStatus.Open);
        result.CreatedBy.Should().Be("test-user-123");
        result.Requestor.Should().Be("requester@example.com");

        var savedCase = await context.Cases.FirstOrDefaultAsync(c => c.Id == result.Id);
        savedCase.Should().NotBeNull();
        savedCase!.Title.Should().Be("Test Case");
    }

    [Fact]
    public async Task HandleWhenUserContextIsNullShouldFallbackToSystem()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);
        
        var mapper = new MapperConfiguration((cfg => cfg.AddProfile<Application.Common.Mappings.MappingProfile>()), NullLoggerFactory.Instance).CreateMapper();

        var userContextMock = new Mock<IUserContext>();
        userContextMock.Setup(u => u.UserId).Returns((string?)null);

        var handler = new CreateCaseCommandHandler(context, mapper, userContextMock.Object);

        var command = new CreateCaseCommand(
            "Fallback Case",
            null,
            Priority.Low,
            CaseType.ServiceRequest,
            "fallback@example.com"
        );

        var result = await handler.Handle(command, CancellationToken.None);

        result.CreatedBy.Should().Be("system");
    }
}
