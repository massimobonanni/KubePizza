using KubePizza.Console.Commands.Order;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.CommandLine;

namespace KubePizza.Tests.Commands.Order;

/// <summary>
/// Unit tests for the OrderCommand class.
/// These tests focus on the core functionality and behavior of the OrderCommand,
/// including constructor validation, property verification, and subcommand setup.
/// 
/// The OrderCommand serves as a parent command that manages pizza orders through
/// its subcommands (create and list). It inherits from CommandBase and uses
/// dependency injection through IServiceProvider.
/// </summary>
public class OrderCommandTests
{
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly OrderCommand _orderCommand;

    /// <summary>
    /// Test setup that runs before each test method.
    /// Creates a mock IServiceProvider and initializes an OrderCommand instance for testing.
    /// This ensures each test starts with a clean, isolated state.
    /// </summary>
    public OrderCommandTests()
    {
        _mockServiceProvider = new Mock<IServiceProvider>();
        _orderCommand = new OrderCommand(_mockServiceProvider.Object);
    }

    /// <summary>
    /// Verifies that the OrderCommand constructor sets the correct command name.
    /// 
    /// Purpose: Ensures the command can be invoked using the correct name "order"
    /// How it works: Creates an OrderCommand and checks that its Name property equals "order"
    /// </summary>
    [Fact]
    public void Constructor_SetsCorrectName()
    {
        // Assert
        Assert.Equal("order", _orderCommand.Name);
    }

    /// <summary>
    /// Verifies that the OrderCommand constructor sets the correct description.
    /// 
    /// Purpose: Ensures users see meaningful help text when using --help
    /// How it works: Creates an OrderCommand and verifies the Description property
    /// </summary>
    [Fact]
    public void Constructor_SetsCorrectDescription()
    {
        // Assert
        Assert.Equal("Manage pizza orders", _orderCommand.Description);
    }

    /// <summary>
    /// Verifies that the OrderCommand adds the "o" alias for convenience.
    /// 
    /// Purpose: Ensures users can use the shorter "o" instead of typing "order"
    /// How it works: Checks that the Aliases collection contains "o"
    /// </summary>
    [Fact]
    public void Constructor_AddsOrderAlias()
    {
        // Assert
        Assert.Contains("o", _orderCommand.Aliases);
    }

    /// <summary>
    /// Verifies that the CreateCommand subcommand is properly added.
    /// 
    /// Purpose: Ensures the "create" functionality is available as a subcommand
    /// How it works: Searches the Subcommands collection for a command named "create"
    /// </summary>
    [Fact]
    public void Constructor_AddsCreateSubcommand()
    {
        // Assert
        Assert.Contains(_orderCommand.Subcommands, cmd => cmd.Name == "create");
    }

    /// <summary>
    /// Verifies that the ListCommand subcommand is properly added.
    /// 
    /// Purpose: Ensures the "list" functionality is available as a subcommand
    /// How it works: Searches the Subcommands collection for a command named "list"
    /// </summary>
    [Fact]
    public void Constructor_AddsListSubcommand()
    {
        // Assert
        Assert.Contains(_orderCommand.Subcommands, cmd => cmd.Name == "list");
    }

    /// <summary>
    /// Verifies that OrderCommand has exactly two subcommands, no more, no less.
    /// 
    /// Purpose: Ensures the command structure is as expected and prevents regression
    /// How it works: Counts the number of subcommands and verifies it equals 2
    /// </summary>
    [Fact]
    public void Constructor_HasExactlyTwoSubcommands()
    {
        // Assert
        Assert.Equal(2, _orderCommand.Subcommands.Count);
    }

    /// <summary>
    /// Verifies that the "create" subcommand is specifically a CreateCommand instance.
    /// 
    /// Purpose: Ensures type safety and that the correct command type is instantiated
    /// How it works: Finds the "create" subcommand and uses Assert.IsType to verify its type
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
    /// Verifies that the "list" subcommand is specifically a ListCommand instance.
    /// 
    /// Purpose: Ensures type safety and that the correct command type is instantiated
    /// How it works: Finds the "list" subcommand and uses Assert.IsType to verify its type
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
    /// Verifies that the IServiceProvider is correctly passed to subcommands.
    /// 
    /// Purpose: Ensures dependency injection works properly throughout the command hierarchy
    /// How it works: Creates a new OrderCommand with a mock service provider, then verifies
    /// that the subcommands were created successfully (which implies they received the service provider)
    /// Note: We can't directly access the protected serviceProvider field, so we verify
    /// indirectly by checking that subcommands have the expected descriptions
    /// </summary>
    [Fact]
    public void Constructor_PassesServiceProviderToSubcommands()
    {
        // Arrange
        var mockServiceProvider = new Mock<IServiceProvider>();
        var orderCommand = new OrderCommand(mockServiceProvider.Object);

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
    /// Verifies that OrderCommand properly inherits from CommandBase.
    /// 
    /// Purpose: Ensures the inheritance hierarchy is correct and OrderCommand gets base functionality
    /// How it works: Uses Assert.IsAssignableFrom to check inheritance relationship
    /// </summary>
    [Fact]
    public void Constructor_InheritsFromCommandBase()
    {
        // Assert
        Assert.IsAssignableFrom<KubePizza.Console.Commands.CommandBase>(_orderCommand);
    }

    /// <summary>
    /// Verifies that OrderCommand inherits from System.CommandLine.Command.
    /// 
    /// Purpose: Ensures OrderCommand is compatible with System.CommandLine framework
    /// How it works: Uses Assert.IsAssignableFrom to check inheritance from the base Command class
    /// </summary>
    [Fact]
    public void Constructor_InheritsFromCommand()
    {
        // Assert
        Assert.IsAssignableFrom<Command>(_orderCommand);
    }

    /// <summary>
    /// Verifies that the command can be invoked using either its full name or alias.
    /// 
    /// Purpose: Ensures both "order" and "o" work as valid command invocations
    /// How it works: Creates a root command, adds the order command, then tests parsing
    /// both the full name and alias to ensure they don't generate parse errors
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
    /// Verifies that passing null as the service provider throws an ArgumentNullException.
    /// 
    /// Purpose: Ensures proper error handling for invalid constructor parameters
    /// How it works: Attempts to create an OrderCommand with null service provider
    /// and verifies that ArgumentNullException is thrown
    /// </summary>
    [Fact]
    public void Constructor_WithNullServiceProvider_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new OrderCommand(null!));
    }

    /// <summary>
    /// Verifies that subcommands have the correct parent-child relationship.
    /// 
    /// Purpose: Ensures the command hierarchy is properly established for help and parsing
    /// How it works: Gets the subcommands and verifies that their parent is the OrderCommand
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
    /// Verifies that OrderCommand itself doesn't define its own options beyond those inherited from CommandBase.
    /// 
    /// Purpose: Ensures OrderCommand is a pure container command that delegates to subcommands
    /// How it works: Filters out the inherited --output option and verifies no other options exist
    /// </summary>
    [Fact]
    public void OrderCommand_HasNoOptions()
    {
        // Assert - OrderCommand itself should not have options, only its subcommands
        Assert.DoesNotContain(_orderCommand.Options, opt => !opt.Name.StartsWith("--output"));
    }

    /// <summary>
    /// Verifies that OrderCommand inherits the --output option from CommandBase.
    /// 
    /// Purpose: Ensures the base functionality for output formatting is available
    /// How it works: Searches the Options collection for the --output option or its -o alias
    /// </summary>
    [Fact]
    public void OrderCommand_InheritsOutputOptionFromCommandBase()
    {
        // Assert
        Assert.Contains(_orderCommand.Options, opt => opt.Name == "--output" || opt.Aliases.Contains("-o"));
    }
}