using CaseOSaurus.Application.Commands.UpdateCaseStatus;
using CaseOSaurus.Application.Common.Exceptions;
using CaseOSaurus.Domain.Entities;
using CaseOSaurus.Domain.Enums;
using CaseOSaurus.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

namespace CaseOSaurus.UnitTests.Commands.UpdateCaseStatus;

public class UpdateCaseStatusCommandHandlerTests
{
    [Fact]
    public async Task HandleValidTransitionShouldUpdateAndReturnResponse()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);

        var caseEntity = new UserCase(
            "Test",
            "Desc",
            Priority.Medium,
            CaseType.Incident,
            "requester@example.com",
            "creator"
        );
        context.Cases.Add(caseEntity);
        await context.SaveChangesAsync();

        var handler = new UpdateCaseStatusCommandHandler(context);
        var command = new UpdateCaseStatusCommand(caseEntity.Id, CaseStatus.InProgress);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Status.Should().Be(CaseStatus.InProgress);
        var updated = await context.Cases.FirstAsync(c => c.Id == caseEntity.Id);
        updated.Status.Should().Be(CaseStatus.InProgress);
    }

    [Fact]
    public async Task HandleInvalidTransitionShouldThrowBusinessRuleException()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);

        var caseEntity = new UserCase(
            "Test",
            "Desc",
            Priority.Medium,
            CaseType.Incident,
            "requester@example.com",
            "creator"
        );
        // Set status to Closed (simulate)
        caseEntity.ChangeStatus(CaseStatus.Closed);
        context.Cases.Add(caseEntity);
        await context.SaveChangesAsync();

        var handler = new UpdateCaseStatusCommandHandler(context);
        var command = new UpdateCaseStatusCommand(caseEntity.Id, CaseStatus.InProgress);

        await Assert.ThrowsAsync<BusinessRuleException>(
            () => handler.Handle(command, CancellationToken.None)
        );
    }

    [Fact]
    public async Task HandleCaseNotFoundShouldThrowNotFoundException()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);
        var handler = new UpdateCaseStatusCommandHandler(context);
        var command = new UpdateCaseStatusCommand(Guid.NewGuid(), CaseStatus.Closed);

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(command, CancellationToken.None)
        );
    }
}
