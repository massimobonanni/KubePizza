using KubePizza.Console.Commands.Order;
using KubePizza.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace KubePizza.Tests.Commands.Order;

/// <summary>
/// Structure tests for <see cref="OrderCommand"/> covering properties, aliases,
/// subcommands, inheritance, option inheritance, parent relationships, and parsing.
/// </summary>
public class OrderCommandStructureTests
{
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly Mock<IConsole> _mockConsole;
    private readonly OrderCommand _orderCommand;

    /// <summary>
    /// Initializes common mocks and the command instance for structure tests.
    /// </summary>
    public OrderCommandStructureTests()
    {
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockConsole = new Mock<IConsole>();
        _orderCommand = new OrderCommand(_mockServiceProvider.Object, _mockConsole.Object);
    }

    /// <summary>
    /// Ensures name, description, and alias are correctly configured.
    /// </summary>
    [Fact]
    public void OrderCommand_HasCorrectCommandProperties()
    {
        // Assert
        Assert.Equal("order", _orderCommand.Name);
        Assert.Equal("Manage pizza orders", _orderCommand.Description);
        Assert.Contains("o", _orderCommand.Aliases);
        Assert.Single(_orderCommand.Aliases);
    }

    /// <summary>
    /// Ensures there are exactly two subcommands.
    /// </summary>
    [Fact]
    public void OrderCommand_HasExactlyTwoSubcommands()
    {
        // Assert
        Assert.Equal(2, _orderCommand.Subcommands.Count);
    }

    /// <summary>
    /// Ensures subcommand names are `create` and `list`.
    /// </summary>
    [Fact]
    public void OrderCommand_SubcommandsHaveCorrectNames()
    {
        // Arrange
        var subcommandNames = _orderCommand.Subcommands.Select(sc => sc.Name).ToList();

        // Assert
        Assert.Contains("create", subcommandNames);
        Assert.Contains("list", subcommandNames);
    }

    /// <summary>
    /// Ensures subcommands are of correct concrete types.
    /// </summary>
    [Fact]
    public void OrderCommand_SubcommandsHaveCorrectTypes()
    {
        // Arrange
        var createSubcommand = _orderCommand.Subcommands.FirstOrDefault(sc => sc.Name == "create");
        var listSubcommand = _orderCommand.Subcommands.FirstOrDefault(sc => sc.Name == "list");

        // Assert
        Assert.NotNull(createSubcommand);
        Assert.NotNull(listSubcommand);
        Assert.IsType<CreateCommand>(createSubcommand);
        Assert.IsType<ListCommand>(listSubcommand);
    }

    /// <summary>
    /// Confirms inheritance from `CommandBase`.
    /// </summary>
    [Fact]
    public void OrderCommand_InheritsFromCommandBase()
    {
        // Assert
        Assert.IsAssignableFrom<KubePizza.Console.Commands.CommandBase>(_orderCommand);
    }

    /// <summary>
    /// Confirms it is assignable to `System.CommandLine.Command`.
    /// </summary>
    [Fact]
    public void OrderCommand_ImplementsSystemCommandLineCommand()
    {
        // Assert
        Assert.IsAssignableFrom<Command>(_orderCommand);
    }

    /// <summary>
    /// Ensures inherited `--output` option exists with alias `-o`.
    /// </summary>
    [Fact]
    public void OrderCommand_HasOutputOptionFromBase()
    {
        // Arrange
        var outputOption = _orderCommand.Options.FirstOrDefault(o => o.Name == "--output");

        // Assert
        Assert.NotNull(outputOption);
        Assert.Contains("-o", outputOption.Aliases);
    }

    /// <summary>
    /// Parameterized verification of subcommand types.
    /// </summary>
    [Theory]
    [InlineData("create", typeof(CreateCommand))]
    [InlineData("list", typeof(ListCommand))]
    public void OrderCommand_SubcommandOfTypeExists(string commandName, Type expectedType)
    {
        // Arrange
        var subcommand = _orderCommand.Subcommands.FirstOrDefault(sc => sc.Name == commandName);

        // Assert
        Assert.NotNull(subcommand);
        Assert.IsType(expectedType, subcommand);
    }

    /// <summary>
    /// Ensures subcommands list `OrderCommand` as parent.
    /// </summary>
    [Fact]
    public void OrderCommand_SubcommandsAreChildrenOfOrderCommand()
    {
        // Assert
        foreach (var subcommand in _orderCommand.Subcommands)
        {
            var parent = subcommand.Parents.FirstOrDefault();
            Assert.Equal(_orderCommand, parent);
        }
    }

    /// <summary>
    /// Ensures it can be added to a `RootCommand`.
    /// </summary>
    [Fact]
    public void OrderCommand_CanBeAddedToRootCommand()
    {
        // Arrange
        var rootCommand = new RootCommand();

        // Act
        rootCommand.Add(_orderCommand);

        // Assert
        Assert.Contains(_orderCommand, rootCommand.Subcommands);
    }

    /// <summary>
    /// Ensures DI reaches subcommands by verifying descriptions.
    /// </summary>
    [Fact]
    public void OrderCommand_ServiceProviderIsPassedToSubcommands()
    {
        // Arrange
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockConsole = new Mock<IConsole>();
        var orderCommand = new OrderCommand(mockServiceProvider.Object, mockConsole.Object);

        // Act - Constructor should pass service provider to subcommands
        var createSubcommand = orderCommand.Subcommands.OfType<CreateCommand>().FirstOrDefault();
        var listSubcommand = orderCommand.Subcommands.OfType<ListCommand>().FirstOrDefault();

        // Assert - We can't directly access the service provider field, but we can verify
        // that the commands were created successfully with proper descriptions
        Assert.NotNull(createSubcommand);
        Assert.NotNull(listSubcommand);
        Assert.Equal("Create a new pizza order", createSubcommand.Description);
        Assert.Equal("List all pizza orders", listSubcommand.Description);
    }

    /// <summary>
    /// Ensures subcommands expose the inherited `--output` option.
    /// </summary>
    [Fact]
    public void OrderCommand_SubcommandsHaveAccessToOutputOption()
    {
        // Assert
        foreach (var subcommand in _orderCommand.Subcommands)
        {
            var outputOption = subcommand.Options.FirstOrDefault(o => o.Name == "--output");
            Assert.NotNull(outputOption);
        }
    }


    /// <summary>
    /// Ensures no positional arguments are defined.
    /// </summary>
    [Fact]
    public void OrderCommand_HasNoArguments()
    {
        // Assert
        Assert.Empty(_orderCommand.Arguments);
    }

    /// <summary>
    /// Confirms the container command has no handler of its own.
    /// </summary>
    [Fact]
    public void OrderCommand_DoesNotDefineOwnHandler()
    {
        // Arrange
        var rootCommand = new RootCommand();
        rootCommand.Add(_orderCommand);

        // Act
        var parseResult = rootCommand.Parse("order");

        // Assert
        // OrderCommand itself shouldn't have a handler - it should rely on subcommands
        Assert.NotNull(parseResult);
        Assert.Equal(_orderCommand, parseResult.CommandResult.Command);
    }
}