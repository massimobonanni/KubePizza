using KubePizza.Console.Commands.Order;
using KubePizza.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace KubePizza.Tests.Commands.Order;

/// <summary>
/// Integration tests for the CreateCommand class.
/// These tests focus on how CreateCommand integrates with the System.CommandLine framework,
/// including command parsing, validation execution, completion functionality, and service integration.
/// 
/// Unlike unit tests that test isolated functionality, these integration tests verify
/// that CreateCommand works correctly within the broader context of a command-line application.
/// They test real parsing scenarios that users would experience when using the CLI.
/// </summary>
public class CreateCommandIntegrationTests
{
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly Mock<IPizzaCatalog> _mockPizzaCatalog;

    /// <summary>
    /// Test setup that runs before each test method.
    /// Creates mock dependencies with realistic test data that simulates a real pizza catalog.
    /// This provides a controlled environment for integration testing.
    /// </summary>
    public CreateCommandIntegrationTests()
    {
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockPizzaCatalog = new Mock<IPizzaCatalog>();

        // Setup realistic test data
        _mockPizzaCatalog.Setup(pc => pc.Pizzas)
       .Returns(new[] { "margherita", "diavola", "capricciosa", "quattroformaggi", "vegetariana" });

        _mockPizzaCatalog.Setup(pc => pc.AllToppings)
  .Returns(new[] { "basil", "mozzarella", "bufala", "olive", "mushrooms", "onions", "peppers", "anchovies", "artichokes", "ham", "salami", "chili" });

        _mockPizzaCatalog.Setup(pc => pc.GetRecommendedToppingsFor("margherita"))
          .Returns(new[] { "basil", "mozzarella", "bufala" });

        _mockPizzaCatalog.Setup(pc => pc.GetRecommendedToppingsFor("diavola"))
            .Returns(new[] { "mozzarella", "chili", "onions" });

        _mockServiceProvider.Setup(sp => sp.GetService(typeof(IPizzaCatalog)))
       .Returns(_mockPizzaCatalog.Object);
    }

    /// <summary>
    /// Verifies that parsing "create --pizza margherita" correctly parses without errors.
    /// 
    /// Purpose: Tests the basic command parsing functionality with valid input
    /// How it works: 
    /// 1. Creates a root command with CreateCommand as a subcommand
    /// 2. Parses a valid command string
    /// 3. Verifies that the parsed result has no errors
    /// 4. Confirms that the resulting command is the CreateCommand
    /// </summary>
    [Fact]
    public void Parse_CreateCommandWithValidPizza_ParsesCorrectly()
    {
        // Arrange
        var createCommand = new CreateCommand(_mockServiceProvider.Object);
        var rootCommand = new RootCommand();
        rootCommand.Add(createCommand);

        // Act
        var parseResult = rootCommand.Parse("create --pizza margherita");

        // Assert
        Assert.NotNull(parseResult);
        Assert.Empty(parseResult.Errors);
        Assert.Equal("create", parseResult.CommandResult.Command.Name);
    }

    /// <summary>
    /// Verifies that parsing with an invalid pizza type generates appropriate errors.
    /// 
    /// Purpose: Tests error handling for invalid user input
    /// How it works:
    /// 1. Creates a root command with CreateCommand
    /// 2. Attempts to parse with an invalid pizza type
    /// 3. Verifies that parsing generates validation errors
    /// </summary>
    [Fact]
    public void Parse_CreateCommandWithInvalidPizza_ReturnsError()
    {
        // Arrange
        var createCommand = new CreateCommand(_mockServiceProvider.Object);
        var rootCommand = new RootCommand();
        rootCommand.Add(createCommand);

        // Act
        var parseResult = rootCommand.Parse("create --pizza invalidpizza");

        // Assert
        Assert.NotNull(parseResult);
        Assert.NotEmpty(parseResult.Errors);
    }

    /// <summary>
    /// Verifies that parsing without the required pizza option generates errors.
    /// 
    /// Purpose: Tests validation for required parameters
    /// How it works:
    /// 1. Creates a root command with CreateCommand
    /// 2. Attempts to parse without the required --pizza option
    /// 3. Verifies that parsing generates required parameter errors
    /// </summary>
    [Fact]
    public void Parse_CreateCommandWithoutRequiredPizza_ReturnsError()
    {
        // Arrange
        var createCommand = new CreateCommand(_mockServiceProvider.Object);
        var rootCommand = new RootCommand();
        rootCommand.Add(createCommand);

        // Act
        var parseResult = rootCommand.Parse("create --size large");

        // Assert
        Assert.NotNull(parseResult);
        Assert.NotEmpty(parseResult.Errors);
    }

    /// <summary>
    /// Verifies that parsing with multiple options works correctly.
    /// 
    /// Purpose: Tests complex command parsing with multiple options
    /// How it works:
    /// 1. Creates a root command with CreateCommand
    /// 2. Parses a command line with multiple valid options
    /// 3. Verifies successful parsing without errors
    /// </summary>
    [Fact]
    public void Parse_CreateCommandWithMultipleOptions_ParsesCorrectly()
    {
        // Arrange
        var createCommand = new CreateCommand(_mockServiceProvider.Object);
        var rootCommand = new RootCommand();
        rootCommand.Add(createCommand);

        // Act
        var parseResult = rootCommand.Parse("create --pizza margherita --size large --toppings basil,mozzarella --delivery true --output json");

        // Assert
        Assert.NotNull(parseResult);
        Assert.Empty(parseResult.Errors);
    }

    /// <summary>
    /// Verifies that help requests parse correctly without errors.
    /// 
    /// Purpose: Tests that the help system integration works properly
    /// How it works:
    /// 1. Creates a root command with CreateCommand
    /// 2. Parses a help request command line
    /// 3. Verifies that help requests don't generate parsing errors
    /// </summary>
    [Fact]
    public void Parse_CreateCommandHelp_ParsesWithoutErrors()
    {
        // Arrange
        var createCommand = new CreateCommand(_mockServiceProvider.Object);
        var rootCommand = new RootCommand();
        rootCommand.Add(createCommand);

        // Act
        var parseResult = rootCommand.Parse("create --help");

        // Assert
        Assert.NotNull(parseResult);
        Assert.Empty(parseResult.Errors);
    }

    /// <summary>
    /// Verifies that the CreateCommand integrates correctly with its parent OrderCommand.
    /// 
    /// Purpose: Tests parent-child relationship in command hierarchy
    /// How it works:
    /// 1. Creates an OrderCommand which should contain CreateCommand
    /// 2. Verifies CreateCommand is present as a subcommand
    /// 3. Verifies the parent-child relationship is established
    /// </summary>
    [Fact]
    public void CreateCommand_IntegratesWithParentOrderCommand()
    {
        // Arrange
        var orderCommand = new OrderCommand(_mockServiceProvider.Object);

        // Act
        var createSubcommand = orderCommand.Subcommands.FirstOrDefault(c => c.Name == "create");

        // Assert
        Assert.NotNull(createSubcommand);
        Assert.IsType<CreateCommand>(createSubcommand);
        Assert.Equal(orderCommand, createSubcommand.Parents.FirstOrDefault());
    }

    /// <summary>
    /// Verifies that the pizza validator integrates with the pizza catalog service.
    /// 
    /// Purpose: Tests service integration for validation
    /// How it works:
    /// 1. Creates a CreateCommand and parses with a valid pizza
    /// 2. Verifies that the service provider was called to get the pizza catalog
    /// 3. This ensures the validator uses the injected service
    /// </summary>
    [Fact]
    public void CreateCommand_PizzaValidator_IntegratesWithPizzaCatalog()
    {
        // Arrange
        var createCommand = new CreateCommand(_mockServiceProvider.Object);
        var rootCommand = new RootCommand();
        rootCommand.Add(createCommand);

        // Act
        var parseResult = rootCommand.Parse("create --pizza margherita");

        // Assert
        _mockServiceProvider.Verify(sp => sp.GetService(typeof(IPizzaCatalog)), Times.AtLeastOnce);
    }

    /// <summary>
    /// Verifies that size validation accepts only valid values.
    /// 
    /// Purpose: Tests integrated validation for size option
    /// How it works:
    /// 1. Tests multiple valid size values using Theory/InlineData
    /// 2. For each value, verifies parsing succeeds without errors
    /// </summary>
    [Theory]
    [InlineData("small")]
    [InlineData("medium")]
    [InlineData("large")]
    public void Parse_CreateCommandWithValidSize_ParsesCorrectly(string size)
    {
        // Arrange
        var createCommand = new CreateCommand(_mockServiceProvider.Object);
        var rootCommand = new RootCommand();
        rootCommand.Add(createCommand);

        // Act
        var parseResult = rootCommand.Parse($"create --pizza margherita --size {size}");

        // Assert
        Assert.NotNull(parseResult);
        Assert.Empty(parseResult.Errors);
    }

    /// <summary>
    /// Verifies that size validation rejects invalid values.
    /// 
    /// Purpose: Tests validation error handling for size option
    /// How it works:
    /// 1. Attempts to parse with an invalid size value
    /// 2. Verifies that parsing generates validation errors
    /// </summary>
    [Fact]
    public void Parse_CreateCommandWithInvalidSize_ReturnsError()
    {
        // Arrange
        var createCommand = new CreateCommand(_mockServiceProvider.Object);
        var rootCommand = new RootCommand();
        rootCommand.Add(createCommand);

        // Act
        var parseResult = rootCommand.Parse("create --pizza margherita --size extralarge");

        // Assert
        Assert.NotNull(parseResult);
        Assert.NotEmpty(parseResult.Errors);
    }

    /// <summary>
    /// Verifies that pizza validation is case-insensitive.
    /// 
    /// Purpose: Tests that users can use different case variations
    /// How it works:
    /// 1. Tests multiple case variations using Theory/InlineData
    /// 2. Verifies all variations parse successfully
    /// </summary>
    [Theory]
    [InlineData("margherita")]
    [InlineData("MARGHERITA")]
    [InlineData("Margherita")]
    [InlineData("MaRgHeRiTa")]
    public void Parse_CreateCommandWithDifferentCasing_ParsesCorrectly(string pizzaName)
    {
        // Arrange
        var createCommand = new CreateCommand(_mockServiceProvider.Object);
        var rootCommand = new RootCommand();
        rootCommand.Add(createCommand);

        // Act
        var parseResult = rootCommand.Parse($"create --pizza {pizzaName}");

        // Assert
        Assert.NotNull(parseResult);
        Assert.Empty(parseResult.Errors);
    }

    /// <summary>
    /// Verifies that toppings can be specified using comma-separated values.
    /// 
    /// Purpose: Tests the custom toppings parser integration
    /// How it works:
    /// 1. Parses a command with comma-separated toppings
    /// 2. Verifies parsing succeeds without errors
    /// </summary>
    [Fact]
    public void Parse_CreateCommandWithCommaSeparatedToppings_ParsesCorrectly()
    {
        // Arrange
        var createCommand = new CreateCommand(_mockServiceProvider.Object);
        var rootCommand = new RootCommand();
        rootCommand.Add(createCommand);

        // Act
        var parseResult = rootCommand.Parse("create --pizza margherita --toppings basil,mozzarella,olive");

        // Assert
        Assert.NotNull(parseResult);
        Assert.Empty(parseResult.Errors);
    }

    /// <summary>
    /// Verifies that toppings can be specified using multiple --toppings arguments.
    /// 
    /// Purpose: Tests multiple argument handling for toppings
    /// How it works:
    /// 1. Parses a command with multiple --toppings arguments
    /// 2. Verifies parsing succeeds without errors
    /// </summary>
    [Fact]
    public void Parse_CreateCommandWithMultipleToppingsArguments_ParsesCorrectly()
    {
        // Arrange
        var createCommand = new CreateCommand(_mockServiceProvider.Object);
        var rootCommand = new RootCommand();
        rootCommand.Add(createCommand);

        // Act
        var parseResult = rootCommand.Parse("create --pizza margherita --toppings basil --toppings mozzarella");

        // Assert
        Assert.NotNull(parseResult);
        Assert.Empty(parseResult.Errors);
    }

    /// <summary>
    /// Verifies that different output formats are handled correctly.
    /// 
    /// Purpose: Tests integration with the inherited output option
    /// How it works:
    /// 1. Tests multiple output format values using Theory/InlineData
    /// 2. Verifies each format parses successfully
    /// </summary>
    [Theory]
    [InlineData("table")]
    [InlineData("json")]
    [InlineData("yaml")]
    public void Parse_CreateCommandWithDifferentOutputFormats_ParsesCorrectly(string outputFormat)
    {
        // Arrange
        var createCommand = new CreateCommand(_mockServiceProvider.Object);
        var rootCommand = new RootCommand();
        rootCommand.Add(createCommand);

        // Act
        var parseResult = rootCommand.Parse($"create --pizza margherita --output {outputFormat}");

        // Assert
        Assert.NotNull(parseResult);
        Assert.Empty(parseResult.Errors);
    }

    /// <summary>
    /// Verifies that invalid output formats are rejected.
    /// 
    /// Purpose: Tests validation for output option values
    /// How it works:
    /// 1. Attempts to parse with an invalid output format
    /// 2. Verifies that parsing generates validation errors
    /// </summary>
    [Fact]
    public void Parse_CreateCommandWithInvalidOutputFormat_ReturnsError()
    {
        // Arrange
        var createCommand = new CreateCommand(_mockServiceProvider.Object);
        var rootCommand = new RootCommand();
        rootCommand.Add(createCommand);

        // Act
        var parseResult = rootCommand.Parse("create --pizza margherita --output invalid");

        // Assert
        Assert.NotNull(parseResult);
        Assert.NotEmpty(parseResult.Errors);
    }

    /// <summary>
    /// Verifies that the delivery option accepts boolean values.
    /// 
    /// Purpose: Tests boolean option parsing
    /// How it works:
    /// 1. Tests both true and false values for delivery option
    /// 2. Verifies both parse successfully
    /// </summary>
    [Theory]
    [InlineData("true")]
    [InlineData("false")]
    public void Parse_CreateCommandWithDeliveryOption_ParsesCorrectly(string deliveryValue)
    {
        // Arrange
        var createCommand = new CreateCommand(_mockServiceProvider.Object);
        var rootCommand = new RootCommand();
        rootCommand.Add(createCommand);

        // Act
        var parseResult = rootCommand.Parse($"create --pizza margherita --delivery {deliveryValue}");

        // Assert
        Assert.NotNull(parseResult);
        Assert.Empty(parseResult.Errors);
    }

    /// <summary>
    /// Verifies that the command can be parsed from within a parent command structure.
    /// 
    /// Purpose: Tests hierarchical command parsing
    /// How it works:
    /// 1. Creates a full command hierarchy (root -> order -> create)
    /// 2. Parses the complete command path
    /// 3. Verifies correct command resolution
    /// </summary>
    [Fact]
    public void Parse_CreateCommandInHierarchy_ParsesCorrectly()
    {
        // Arrange
        var orderCommand = new OrderCommand(_mockServiceProvider.Object);
        var rootCommand = new RootCommand();
        rootCommand.Add(orderCommand);

        // Act
        var parseResult = rootCommand.Parse("order create --pizza margherita");

        // Assert
        Assert.NotNull(parseResult);
        Assert.Empty(parseResult.Errors);
        Assert.Equal("create", parseResult.CommandResult.Command.Name);
    }
}
