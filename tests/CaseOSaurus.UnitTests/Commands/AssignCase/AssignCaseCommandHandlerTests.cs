using CaseOSaurus.Application.Commands.AssignCase;
using CaseOSaurus.Application.Common.Exceptions;
using CaseOSaurus.Domain.Entities;
using CaseOSaurus.Domain.Enums;
using CaseOSaurus.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

namespace CaseOSaurus.UnitTests.Commands.AssignCase;

public class AssignCaseCommandHandlerTests
{
    [Fact]
    public async Task HandleWithValidCommandShouldAssignAndReturnCaseResponse()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);

        // Seed a case
        var testCase = new UserCase(
            "Test Title",
            "Description",
            Priority.Medium,
            CaseType.Incident,
            "requester@example.com",
            "creator-user"
        );
        context.Cases.Add(testCase);
        await context.SaveChangesAsync();

        var handler = new AssignCaseCommandHandler(context);

        var command = new AssignCaseCommand(testCase.Id, "new-assignee-123");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.AssignedTo.Should().Be("new-assignee-123");
        result.Id.Should().Be(testCase.Id);

        // Verify in database
        var updated = await context.Cases.FirstAsync(c => c.Id == testCase.Id);
        updated.AssignedTo.Should().Be("new-assignee-123");
    }

    [Fact]
    public async Task HandleWhenCaseNotFoundShouldThrowNotFoundException()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);
        var handler = new AssignCaseCommandHandler(context);
        var command = new AssignCaseCommand(Guid.NewGuid(), "user-123");

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}
