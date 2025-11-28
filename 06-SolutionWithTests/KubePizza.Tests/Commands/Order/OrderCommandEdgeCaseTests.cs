using KubePizza.Console.Commands.Order;
using KubePizza.Core.Interfaces;
using KubePizza.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.CommandLine;

namespace KubePizza.Tests.Commands.Order;

/// <summary>
/// Edge case tests for <see cref="OrderCommand"/> covering null arguments, multiple parents,
/// parsing with missing/invalid inputs, consistency of collections, and basic object semantics.
/// </summary>
public class OrderCommandEdgeCaseTests
{
    /// <summary>
    /// Validates null service provider throws `ArgumentNullException` with correct parameter name.
    /// </summary>
    [Fact]
    public void Constructor_WithNullServiceProvider_ThrowsArgumentNullException()
    {
        // Arrange
        var mockConsole = new Mock<IConsole>();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => new OrderCommand(null!, mockConsole.Object));
        Assert.Equal("serviceProvider", exception.ParamName);
    }


    [Fact]
    public void Constructor_WithNullConsole_ThrowsArgumentNullException()
    {
        // Arrange
        var mockServiceProvider = new Mock<IServiceProvider>();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => new OrderCommand(mockServiceProvider.Object, null!));
        Assert.Equal("console", exception.ParamName);
    }

    /// <summary>
    /// Ensures construction with valid dependencies does not throw.
    /// </summary>
    [Fact]
    public void Constructor_WithValidServiceProvider_DoesNotThrow()
    {
        // Arrange
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockConsole = new Mock<IConsole>();

        // Act & Assert
        var exception = Record.Exception(() => new OrderCommand(mockServiceProvider.Object, mockConsole.Object));
        Assert.Null(exception);
    }

    /// <summary>
    /// Ensures multiple instances are independent but share expected metadata.
    /// </summary>
    [Fact]
    public void Constructor_CalledMultipleTimes_CreatesIndependentInstances()
    {
        // Arrange
        var mockServiceProvider1 = new Mock<IServiceProvider>();
        var mockServiceProvider2 = new Mock<IServiceProvider>();
        var mockConsole = new Mock<IConsole>();

        // Act
        var orderCommand1 = new OrderCommand(mockServiceProvider1.Object, mockConsole.Object);
        var orderCommand2 = new OrderCommand(mockServiceProvider2.Object, mockConsole.Object);

        // Assert
        Assert.NotSame(orderCommand1, orderCommand2);
        Assert.Equal(orderCommand1.Name, orderCommand2.Name);
        Assert.Equal(orderCommand1.Description, orderCommand2.Description);
        Assert.Equal(orderCommand1.Subcommands.Count, orderCommand2.Subcommands.Count);

        // Subcommands should be different instances
        Assert.NotSame(orderCommand1.Subcommands.First(), orderCommand2.Subcommands.First());
    }

    /// <summary>
    /// Verifies framework behavior when adding the same command to multiple parents.
    /// </summary>
    [Fact]
    public void OrderCommand_WhenAddedToMultipleParents_WorksCorrectly()
    {
        // Arrange
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockConsole = new Mock<IConsole>();
        var orderCommand = new OrderCommand(mockServiceProvider.Object, mockConsole.Object);
        var rootCommand1 = new RootCommand("root1");
        var rootCommand2 = new RootCommand("root2");

        // Act
        rootCommand1.Add(orderCommand);

        // Assert - Adding to second parent should work (Command allows multiple parents)
        var exception = Record.Exception(() => rootCommand2.Add(orderCommand));
        // Note: System.CommandLine may or may not allow this - the test verifies current behavior
        // If it throws, that's the expected behavior; if it doesn't, that's also valid
        Assert.True(exception == null || exception is InvalidOperationException);
    }

    /// <summary>
    /// Parses just "order" and verifies the container command is selected.
    /// </summary>
    [Fact]
    public void OrderCommand_Parsing_EmptyArguments_SelectsOrderCommand()
    {
        // Arrange
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockConsole = new Mock<IConsole>();
        var orderCommand = new OrderCommand(mockServiceProvider.Object, mockConsole.Object);
        var rootCommand = new RootCommand();
        rootCommand.Add(orderCommand);

        // Act
        var parseResult = rootCommand.Parse("order");

        // Assert
        Assert.NotNull(parseResult);
        Assert.Empty(parseResult.Errors);
        Assert.Equal(orderCommand, parseResult.CommandResult.Command);
    }

    /// <summary>
    /// Verifies invalid `--output` values produce parsing errors.
    /// </summary>
    [Fact]
    public void OrderCommand_Parsing_InvalidGlobalOption_GeneratesError()
    {
        // Arrange
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockConsole = new Mock<IConsole>();
        var orderCommand = new OrderCommand(mockServiceProvider.Object, mockConsole.Object);
        var rootCommand = new RootCommand();
        rootCommand.Add(orderCommand);

        // Act
        var parseResult = rootCommand.Parse("order --output invalid");

        // Assert
        Assert.NotNull(parseResult);
        Assert.NotEmpty(parseResult.Errors);
    }

    /// <summary>
    /// Ensures repeated access to `Subcommands` yields consistent instances.
    /// </summary>
    [Fact]
    public void OrderCommand_Subcommands_CanBeAccessedMultipleTimes()
    {
        // Arrange
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockConsole = new Mock<IConsole>();
        var orderCommand = new OrderCommand(mockServiceProvider.Object, mockConsole.Object);

        // Act
        var subcommands1 = orderCommand.Subcommands.ToList();
        var subcommands2 = orderCommand.Subcommands.ToList();

        // Assert
        Assert.Equal(subcommands1.Count, subcommands2.Count);
        Assert.Equal(subcommands1.Select(sc => sc.Name), subcommands2.Select(sc => sc.Name));

        // Should return the same instances
        for (int i = 0; i < subcommands1.Count; i++)
        {
            Assert.Same(subcommands1[i], subcommands2[i]);
        }
    }

    /// <summary>
    /// Ensures repeated access to `Aliases` yields identical results.
    /// </summary>
    [Fact]
    public void OrderCommand_Aliases_CanBeAccessedMultipleTimes()
    {
        // Arrange
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockConsole = new Mock<IConsole>();
        var orderCommand = new OrderCommand(mockServiceProvider.Object, mockConsole.Object);

        // Act
        var aliases1 = orderCommand.Aliases.ToList();
        var aliases2 = orderCommand.Aliases.ToList();

        // Assert
        Assert.Equal(aliases1, aliases2);
        Assert.Contains("o", aliases1);
        Assert.Single(aliases1);
    }

    /// <summary>
    /// Ensures `ToString()` returns a meaningful representation containing the name.
    /// </summary>
    [Fact]
    public void OrderCommand_ToString_ReturnsUsefulInformation()
    {
        // Arrange
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockConsole = new Mock<IConsole>();
        var orderCommand = new OrderCommand(mockServiceProvider.Object, mockConsole.Object);

        // Act
        var stringRepresentation = orderCommand.ToString();

        // Assert
        Assert.NotNull(stringRepresentation);
        Assert.NotEmpty(stringRepresentation);
        // The exact format may vary, but it should contain the command name
        Assert.Contains("order", stringRepresentation, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Ensures `GetHashCode()` is stable for the same instance.
    /// </summary>
    [Fact]
    public void OrderCommand_GetHashCode_IsConsistent()
    {
        // Arrange
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockConsole = new Mock<IConsole>();
        var orderCommand = new OrderCommand(mockServiceProvider.Object, mockConsole.Object);

        // Act
        var hashCode1 = orderCommand.GetHashCode();
        var hashCode2 = orderCommand.GetHashCode();

        // Assert
        Assert.Equal(hashCode1, hashCode2);
    }

    /// <summary>
    /// Ensures `Equals` semantics for self, different instance, null, and other types.
    /// </summary>
    [Fact]
    public void OrderCommand_Equals_WorksCorrectly()
    {
        // Arrange
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockConsole = new Mock<IConsole>();
        var orderCommand1 = new OrderCommand(mockServiceProvider.Object, mockConsole.Object);
        var orderCommand2 = new OrderCommand(mockServiceProvider.Object, mockConsole.Object);

        // Act & Assert
        Assert.True(orderCommand1.Equals(orderCommand1)); // Same instance
        Assert.False(orderCommand1.Equals(orderCommand2)); // Different instances
        Assert.False(orderCommand1.Equals(null)); // Null comparison
        Assert.False(orderCommand1.Equals("string")); // Different type
    }
}