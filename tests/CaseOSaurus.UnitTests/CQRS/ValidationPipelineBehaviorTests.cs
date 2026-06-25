using FluentValidation;
using FluentValidation.Results;
using CaseOSaurus.Application.CQRS.Interfaces;
using CaseOSaurus.Application.CQRS.PipelineBehaviors;
using Moq;
using Xunit;
using FluentAssertions;

namespace CaseOSaurus.UnitTests.CQRS;

public class ValidationPipelineBehaviorTests
{
    public record TestCommand : ICommand<string>;
    public class TestValidator : AbstractValidator<TestCommand> { }

    [Fact]
    public async Task HandleWhenValidatorFailsShouldThrowValidationException()
    {
        // Arrange
        var validatorMock = new Mock<IValidator<TestCommand>>();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestCommand>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("Title", "Title is required") }));

        var validators = new[] { validatorMock.Object };
        var behavior = new ValidationPipelineBehavior<TestCommand, string>(validators);

        var request = new TestCommand();
        RequestHandlerDelegate<string> next = () => Task.FromResult("success");

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(
            () => behavior.Handle(request, next, CancellationToken.None)
        );
    }

    [Fact]
    public async Task HandleWhenNoValidatorsShouldCallNext()
    {
        // Arrange
        var behavior = new ValidationPipelineBehavior<TestCommand, string>(Enumerable.Empty<IValidator<TestCommand>>());
        var request = new TestCommand();
        var nextCalled = false;
        RequestHandlerDelegate<string> next = () => { nextCalled = true; return Task.FromResult("success"); };

        // Act
        var result = await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        nextCalled.Should().BeTrue();
        result.Should().Be("success");
    }
}
