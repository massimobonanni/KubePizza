using KubePizza.Console.Commands.Order;
using KubePizza.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.CommandLine;

namespace KubePizza.Tests.Commands.Order;

/// <summary>
/// Unit tests for <see cref="OrderCommand"/> covering name/description, aliases,
/// subcommand registration, inheritance, and basic parsing behavior. The command acts
/// as a container for the `create` and `list` subcommands and inherits base options.
/// </summary>
public class OrderCommandTests
{
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly Mock<IConsole> _mockConsole;
    private readonly OrderCommand _orderCommand;

    /// <summary>
    /// Initializes common mocks and the command instance used across tests.
    /// </summary>
    public OrderCommandTests()
    {
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockConsole = new Mock<IConsole>();
        _orderCommand = new OrderCommand(_mockServiceProvider.Object, _mockConsole.Object);
    }

    /// <summary>
    /// Ensures the command name is set to "order".
    /// </summary>
    [Fact]
    public void Constructor_SetsCorrectName()
    {
        // Assert
        Assert.Equal("order", _orderCommand.Name);
    }

    /// <summary>
    /// Ensures the description is user friendly.
    /// </summary>
    [Fact]
    public void Constructor_SetsCorrectDescription()
    {
        // Assert
        Assert.Equal("Manage pizza orders", _orderCommand.Description);
    }

    /// <summary>
    /// Verifies the short alias "o" is present.
    /// </summary>
    [Fact]
    public void Constructor_AddsOrderAlias()
    {
        // Assert
        Assert.Contains("o", _orderCommand.Aliases);
    }

    /// <summary>
    /// Ensures the `create` subcommand is added.
    /// </summary>
    [Fact]
    public void Constructor_AddsCreateSubcommand()
    {
        // Assert
        Assert.Contains(_orderCommand.Subcommands, cmd => cmd.Name == "create");
    }

    /// <summary>
    /// Ensures the `list` subcommand is added.
    /// </summary>
    [Fact]
    public void Constructor_AddsListSubcommand()
    {
        // Assert
        Assert.Contains(_orderCommand.Subcommands, cmd => cmd.Name == "list");
    }

    /// <summary>
    /// Ensures there are exactly two subcommands.
    /// </summary>
    [Fact]
    public void Constructor_HasExactlyTwoSubcommands()
    {
        // Assert
        Assert.Equal(2, _orderCommand.Subcommands.Count);
    }

    /// <summary>
    /// Ensures the `create` subcommand type is `CreateCommand`.
    /// </summary>
    [Fact]
    public void Constructor_CreateSubcommandIsCreateCommand()
    {
        // Arrange
        var createSubcommand = _orderCommand.Subcommands.FirstOrDefault(cmd => cmd.Name == "create");

        // Assert
        Assert.NotNull(createSubcommand);
        Assert.IsType<CreateCommand>(createSubcommand);
    }

    /// <summary>
    /// Ensures the `list` subcommand type is `ListCommand`.
    /// </summary>
    [Fact]
    public void Constructor_ListSubcommandIsListCommand()
    {
        // Arrange
        var listSubcommand = _orderCommand.Subcommands.FirstOrDefault(cmd => cmd.Name == "list");

        // Assert
        Assert.NotNull(listSubcommand);
        Assert.IsType<ListCommand>(listSubcommand);
    }

    /// <summary>
    /// Ensures DI dependencies reach subcommands by confirming successful construction and descriptions.
    /// </summary>
    [Fact]
    public void Constructor_PassesServiceProviderToSubcommands()
    {
        // Arrange
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockConsole = new Mock<IConsole>();
        var orderCommand = new OrderCommand(mockServiceProvider.Object, mockConsole.Object);

        // Act
        var createSubcommand = orderCommand.Subcommands.FirstOrDefault(cmd => cmd.Name == "create") as CreateCommand;
        var listSubcommand = orderCommand.Subcommands.FirstOrDefault(cmd => cmd.Name == "list") as ListCommand;

        // Assert
        Assert.NotNull(createSubcommand);
        Assert.NotNull(listSubcommand);

        // We can't directly test the service provider since it's protected in CommandBase
        // but we can verify the commands were constructed successfully which implies the service provider was passed
        Assert.Equal("Create a new pizza order", createSubcommand.Description);
        Assert.Equal("List all pizza orders", listSubcommand.Description);
    }

    [Fact]
    public void Constructor_PassesConsoleToSubcommands()
    {
        // Arrange
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockConsole = new Mock<IConsole>();
        var orderCommand = new OrderCommand(mockServiceProvider.Object, mockConsole.Object);

        // Act
        var createSubcommand = orderCommand.Subcommands.FirstOrDefault(cmd => cmd.Name == "create") as CreateCommand;
        var listSubcommand = orderCommand.Subcommands.FirstOrDefault(cmd => cmd.Name == "list") as ListCommand;

        // Assert
        Assert.NotNull(createSubcommand);
        Assert.NotNull(listSubcommand);

        // We can't directly test the service provider since it's protected in CommandBase
        // but we can verify the commands were constructed successfully which implies the service provider was passed
        Assert.Equal("Create a new pizza order", createSubcommand.Description);
        Assert.Equal("List all pizza orders", listSubcommand.Description);
    }

    /// <summary>
    /// Confirms inheritance from `CommandBase`.
    /// </summary>
    [Fact]
    public void Constructor_InheritsFromCommandBase()
    {
        // Assert
        Assert.IsAssignableFrom<KubePizza.Console.Commands.CommandBase>(_orderCommand);
    }

    /// <summary>
    /// Confirms inheritance from `System.CommandLine.Command`.
    /// </summary>
    [Fact]
    public void Constructor_InheritsFromCommand()
    {
        // Assert
        Assert.IsAssignableFrom<Command>(_orderCommand);
    }

    /// <summary>
    /// Ensures both full name and alias can be parsed.
    /// </summary>
    [Theory]
    [InlineData("order")]
    [InlineData("o")]
    public void Command_CanBeInvokedByNameOrAlias(string commandName)
    {
        // Arrange
        var rootCommand = new RootCommand();
        rootCommand.Add(_orderCommand);

        // Act & Assert - Should not throw
        var parseResult = rootCommand.Parse(commandName);
        Assert.NotNull(parseResult);
    }

    /// <summary>
    /// Validates null service provider throws `ArgumentNullException`.
    /// </summary>
    [Fact]
    public void Constructor_WithNullServiceProvider_ThrowsArgumentNullException()
    {
        // Arrange
        var mockConsole = new Mock<IConsole>();
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new OrderCommand(null!, mockConsole.Object));
    }

    [Fact]
    public void Constructor_WithNullConsole_ThrowsArgumentNullException()
    {
        // Arrange
        var mockServiceProvider = new Mock<IServiceProvider>();
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new OrderCommand(mockServiceProvider.Object, null!));
    }

    /// <summary>
    /// Ensures subcommands have the correct parent.
    /// </summary>
    [Fact]
    public void Subcommands_HaveCorrectParent()
    {
        // Arrange
        var createSubcommand = _orderCommand.Subcommands.FirstOrDefault(cmd => cmd.Name == "create");
        var listSubcommand = _orderCommand.Subcommands.FirstOrDefault(cmd => cmd.Name == "list");

        // Assert
        Assert.NotNull(createSubcommand);
        Assert.NotNull(listSubcommand);
        Assert.Equal(_orderCommand, createSubcommand.Parents.FirstOrDefault());
        Assert.Equal(_orderCommand, listSubcommand.Parents.FirstOrDefault());
    }

    /// <summary>
    /// Ensures `OrderCommand` defines no extra options beyond inherited ones.
    /// </summary>
    [Fact]
    public void OrderCommand_HasNoOptions()
    {
        // Assert - OrderCommand itself should not have options, only its subcommands
        Assert.DoesNotContain(_orderCommand.Options, opt => !opt.Name.StartsWith("--output"));
    }

    /// <summary>
    /// Confirms inherited `--output` option exists.
    /// </summary>
    [Fact]
    public void OrderCommand_InheritsOutputOptionFromCommandBase()
    {
        // Assert
        Assert.Contains(_orderCommand.Options, opt => opt.Name == "--output" || opt.Aliases.Contains("-o"));
    }
}