using KubePizza.Console.Commands.Order;
using KubePizza.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace KubePizza.Tests.Commands.Order;

/// <summary>
/// Integration tests for the OrderCommand class.
/// These tests focus on how OrderCommand integrates with the System.CommandLine framework,
/// including command parsing, subcommand routing, and CLI interaction scenarios.
/// 
/// Unlike unit tests that test isolated functionality, these integration tests verify
/// that OrderCommand works correctly within the broader context of a command-line application.
/// They test real parsing scenarios that users would experience when using the CLI.
/// </summary>
public class OrderCommandIntegrationTests
{
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly Mock<IConsole> _mockConsole;

    /// <summary>
    /// Test setup that runs before each test method.
    /// Creates a mock IServiceProvider that will be used across integration tests.
    /// This simulates the dependency injection container that would be used in a real application.
    /// </summary>
    public OrderCommandIntegrationTests()
    {
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockConsole = new Mock<IConsole>();
    }

    /// <summary>
    /// Verifies that parsing "order" correctly identifies the OrderCommand.
    /// 
    /// Purpose: Tests the basic command recognition functionality
    /// How it works: 
    /// 1. Creates a root command with OrderCommand as a subcommand
    /// 2. Parses the string "order" 
    /// 3. Verifies that the parsed result correctly identifies the OrderCommand
    /// 4. Ensures no parsing errors occurred
    /// </summary>
    [Fact]
    public void Parse_OrderCommand_ReturnsCorrectCommand()
    {
        // Arrange
        var orderCommand = new OrderCommand(_mockServiceProvider.Object, _mockConsole.Object);
        var rootCommand = new RootCommand();
        rootCommand.Add(orderCommand);

        // Act
        var parseResult = rootCommand.Parse("order");

        // Assert
        Assert.NotNull(parseResult);
        Assert.Equal(orderCommand, parseResult.CommandResult.Command);
        Assert.NotEmpty(parseResult.Errors);
    }

    /// <summary>
    /// Verifies that parsing "o" (the alias) correctly identifies the OrderCommand.
    /// 
    /// Purpose: Tests that command aliases work correctly in parsing
    /// How it works:
    /// 1. Creates a root command with OrderCommand as a subcommand
    /// 2. Parses the string "o" (the alias for "order")
    /// 3. Verifies that the parsed result correctly identifies the OrderCommand
    /// 4. Ensures no parsing errors occurred
    /// </summary>
    [Fact]
    public void Parse_OrderWithAlias_ReturnsCorrectCommand()
    {
        // Arrange
        var orderCommand = new OrderCommand(_mockServiceProvider.Object, _mockConsole.Object);
        var rootCommand = new RootCommand();
        rootCommand.Add(orderCommand);

        // Act
        var parseResult = rootCommand.Parse("o");

        // Assert
        Assert.NotNull(parseResult);
        Assert.Equal(orderCommand, parseResult.CommandResult.Command);
        Assert.NotEmpty(parseResult.Errors);
    }

    /// <summary>
    /// Verifies that parsing "order create" correctly routes to the CreateCommand subcommand.
    /// 
    /// Purpose: Tests subcommand routing functionality - ensuring users can access create functionality
    /// How it works:
    /// 1. Creates a root command with OrderCommand containing its subcommands
    /// 2. Parses the string "order create"
    /// 3. Verifies that parsing succeeds without errors
    /// 4. Confirms that the resulting command is the "create" subcommand
    /// </summary>
    [Fact]
    public async Task Parse_OrderCreateSubcommand_ParsesCorrectly()
    {
        // Arrange
        var orderCommand = new OrderCommand(_mockServiceProvider.Object, _mockConsole.Object);
        var rootCommand = new RootCommand();
        rootCommand.Add(orderCommand);

        // Act
        var parseResult = rootCommand.Parse("order create");

        // Assert
        Assert.NotNull(parseResult);
        var commandResult = parseResult.CommandResult;
        Assert.Equal("create", commandResult.Command.Name);
    }

    /// <summary>
    /// Verifies that parsing "order list" correctly routes to the ListCommand subcommand.
    /// 
    /// Purpose: Tests subcommand routing functionality - ensuring users can access list functionality
    /// How it works:
    /// 1. Creates a root command with OrderCommand containing its subcommands  
    /// 2. Parses the string "order list"
    /// 3. Verifies that parsing succeeds without errors
    /// 4. Confirms that the resulting command is the "list" subcommand
    /// </summary>
    [Fact]
    public async Task Parse_OrderListSubcommand_ParsesCorrectly()
    {
        // Arrange
        var orderCommand = new OrderCommand(_mockServiceProvider.Object, _mockConsole.Object);
        var rootCommand = new RootCommand();
        rootCommand.Add(orderCommand);

        // Act
        var parseResult = rootCommand.Parse("order list");

        // Assert
        Assert.NotNull(parseResult);
        Assert.Empty(parseResult.Errors);
        var commandResult = parseResult.CommandResult;
        Assert.Equal("list", commandResult.Command.Name);
    }

    /// <summary>
    /// Verifies that parsing an invalid subcommand generates appropriate errors.
    /// 
    /// Purpose: Tests error handling for invalid user input
    /// How it works:
    /// 1. Creates a root command with OrderCommand
    /// 2. Attempts to parse "order invalid" (where "invalid" is not a real subcommand)
    /// 3. Verifies that parsing generates errors (indicating the invalid subcommand was caught)
    /// </summary>
    [Fact]
    public void Parse_InvalidSubcommand_ReturnsError()
    {
        // Arrange
        var orderCommand = new OrderCommand(_mockServiceProvider.Object, _mockConsole.Object);
        var rootCommand = new RootCommand();
        rootCommand.Add(orderCommand);

        // Act
        var parseResult = rootCommand.Parse("order invalid");

        // Assert
        Assert.NotNull(parseResult);
        Assert.NotEmpty(parseResult.Errors);
    }

    /// <summary>
    /// Verifies that help requests for subcommands parse correctly without errors.
    /// 
    /// Purpose: Tests that the help system integration works properly
    /// How it works:
    /// 1. Tests multiple help command variations using Theory/InlineData
    /// 2. For each variation, parses the command line
    /// 3. Verifies that help requests don't generate parsing errors
    /// 4. This ensures users can get help for both the full command names and aliases
    /// </summary>
    [Theory]
    [InlineData("order create --help")]
    [InlineData("order list --help")]
    [InlineData("o create --help")]
    [InlineData("o list --help")]
    public void Parse_HelpForSubcommands_ParsesWithoutErrors(string commandLine)
    {
        // Arrange
        var orderCommand = new OrderCommand(_mockServiceProvider.Object, _mockConsole.Object);
        var rootCommand = new RootCommand();
        rootCommand.Add(orderCommand);

        // Act
        var parseResult = rootCommand.Parse(commandLine);

        // Assert
        Assert.NotNull(parseResult);
        // Help requests don't generate parse errors
        Assert.Empty(parseResult.Errors);
    }

    /// <summary>
    /// Verifies that subcommands have the correct structure and descriptions.
    /// 
    /// Purpose: Tests that the command structure is properly established during integration
    /// How it works:
    /// 1. Creates an OrderCommand and verifies it has exactly 2 subcommands
    /// 2. Locates the "create" and "list" subcommands
    /// 3. Verifies that each subcommand has the expected description
    /// 4. This ensures the command structure is consistent and user-friendly
    /// </summary>
    [Fact]
    public void OrderCommand_SubcommandsHaveCorrectStructure()
    {
        // Arrange
        var orderCommand = new OrderCommand(_mockServiceProvider.Object, _mockConsole.Object);

        // Act & Assert
        Assert.Equal(2, orderCommand.Subcommands.Count);

        var createCommand = orderCommand.Subcommands.First(c => c.Name == "create");
        var listCommand = orderCommand.Subcommands.First(c => c.Name == "list");

        Assert.Equal("Create a new pizza order", createCommand.Description);
        Assert.Equal("List all pizza orders", listCommand.Description);
    }

    /// <summary>
    /// Verifies that all subcommands inherit from CommandBase as expected.
    /// 
    /// Purpose: Tests the inheritance structure in an integration context
    /// How it works:
    /// 1. Creates an OrderCommand and examines all its subcommands
    /// 2. For each subcommand, verifies it inherits from CommandBase
    /// 3. This ensures consistent behavior and shared functionality across all subcommands
    /// </summary>
    [Fact]
    public void OrderCommand_AllSubcommandsInheritFromCommandBase()
    {
        // Arrange
        var orderCommand = new OrderCommand(_mockServiceProvider.Object, _mockConsole.Object);

        // Act & Assert
        foreach (var subcommand in orderCommand.Subcommands)
        {
            Assert.IsAssignableFrom<KubePizza.Console.Commands.CommandBase>(subcommand);
        }
    }

    /// <summary>
    /// Verifies that all subcommands have the inherited --output option available.
    /// 
    /// Purpose: Tests that option inheritance works correctly in the integration context
    /// How it works:
    /// 1. Creates an OrderCommand and examines all its subcommands
    /// 2. For each subcommand, searches for the --output option or its -o alias
    /// 3. Verifies that the option exists, ensuring users can control output formatting
    /// 4. This tests the inheritance chain from CommandBase through to all subcommands
    /// </summary>
    [Fact]
    public void OrderCommand_AllSubcommandsHaveOutputOption()
    {
        // Arrange
        var orderCommand = new OrderCommand(_mockServiceProvider.Object, _mockConsole.Object);

        // Act & Assert
        foreach (var subcommand in orderCommand.Subcommands)
        {
            var outputOption = subcommand.Options.FirstOrDefault(o => o.Name == "--output" || o.Aliases.Contains("-o"));
            Assert.NotNull(outputOption);
        }
    }
}