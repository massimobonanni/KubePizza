using KubePizza.Console.Commands.Order;
using KubePizza.Core.Interfaces;
using KubePizza.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace KubePizza.Tests.Commands.Order;

/// <summary>
/// Integration tests for <see cref="CreateCommand"/> focusing on parsing, validation,
/// completion, service integration, and option behavior.
/// </summary>
public class CreateCommandIntegrationTests
{
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly Mock<IPizzaCatalog> _mockPizzaCatalog;
    private readonly Mock<IConsole> _mockConsole;

    /// <summary>
    /// Initializes common mocks with realistic test data.
    /// </summary>
    public CreateCommandIntegrationTests()
    {
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockPizzaCatalog = new Mock<IPizzaCatalog>();
        _mockConsole = new Mock<IConsole>();

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
    /// Parses a valid command and verifies no errors.
    /// </summary>
    [Fact]
    public void Parse_CreateCommandWithValidPizza_ParsesCorrectly()
    {
        // Arrange
        var createCommand = new CreateCommand(_mockServiceProvider.Object, _mockConsole.Object);
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
    /// Parses with invalid pizza and verifies errors.
    /// </summary>
    [Fact]
    public void Parse_CreateCommandWithInvalidPizza_ReturnsError()
    {
        // Arrange
        var createCommand = new CreateCommand(_mockServiceProvider.Object, _mockConsole.Object);
        var rootCommand = new RootCommand();
        rootCommand.Add(createCommand);

        // Act
        var parseResult = rootCommand.Parse("create --pizza invalidpizza");

        // Assert
        Assert.NotNull(parseResult);
        Assert.NotEmpty(parseResult.Errors);
    }

    /// <summary>
    /// Omitting required `--pizza` produces errors.
    /// </summary>
    [Fact]
    public void Parse_CreateCommandWithoutRequiredPizza_ReturnsError()
    {
        // Arrange
        var createCommand = new CreateCommand(_mockServiceProvider.Object, _mockConsole.Object);
        var rootCommand = new RootCommand();
        rootCommand.Add(createCommand);

        // Act
        var parseResult = rootCommand.Parse("create --size large");

        // Assert
        Assert.NotNull(parseResult);
        Assert.NotEmpty(parseResult.Errors);
    }

    /// <summary>
    /// Parses with multiple valid options and verifies success.
    /// </summary>
    [Fact]
    public void Parse_CreateCommandWithMultipleOptions_ParsesCorrectly()
    {
        // Arrange
        var createCommand = new CreateCommand(_mockServiceProvider.Object, _mockConsole.Object);
        var rootCommand = new RootCommand();
        rootCommand.Add(createCommand);

        // Act
        var parseResult = rootCommand.Parse("create --pizza margherita --size large --toppings basil,mozzarella --delivery true --output json");

        // Assert
        Assert.NotNull(parseResult);
        Assert.Empty(parseResult.Errors);
    }

    /// <summary>
    /// Parses help request and verifies no errors.
    /// </summary>
    [Fact]
    public void Parse_CreateCommandHelp_ParsesWithoutErrors()
    {
        // Arrange
        var createCommand = new CreateCommand(_mockServiceProvider.Object, _mockConsole.Object);
        var rootCommand = new RootCommand();
        rootCommand.Add(createCommand);

        // Act
        var parseResult = rootCommand.Parse("create --help");

        // Assert
        Assert.NotNull(parseResult);
        Assert.Empty(parseResult.Errors);
    }

    /// <summary>
    /// Verifies subcommand registration and parent-child relationship.
    /// </summary>
    [Fact]
    public void CreateCommand_IntegratesWithParentOrderCommand()
    {
        // Arrange
        var orderCommand = new OrderCommand(_mockServiceProvider.Object, _mockConsole.Object);

        // Act
        var createSubcommand = orderCommand.Subcommands.FirstOrDefault(c => c.Name == "create");

        // Assert
        Assert.NotNull(createSubcommand);
        Assert.IsType<CreateCommand>(createSubcommand);
        Assert.Equal(orderCommand, createSubcommand.Parents.FirstOrDefault());
    }

    /// <summary>
    /// Confirms pizza validator calls into the catalog service.
    /// </summary>
    [Fact]
    public void CreateCommand_PizzaValidator_IntegratesWithPizzaCatalog()
    {
        // Arrange
        var createCommand = new CreateCommand(_mockServiceProvider.Object, _mockConsole.Object);
        var rootCommand = new RootCommand();
        rootCommand.Add(createCommand);

        // Act
        var parseResult = rootCommand.Parse("create --pizza margherita");

        // Assert
        _mockServiceProvider.Verify(sp => sp.GetService(typeof(IPizzaCatalog)), Times.AtLeastOnce);
    }

    /// <summary>
    /// Validates accepted size values parse successfully.
    /// </summary>
    [Theory]
    [InlineData("small")]
    [InlineData("medium")]
    [InlineData("large")]
    public void Parse_CreateCommandWithValidSize_ParsesCorrectly(string size)
    {
        // Arrange
        var createCommand = new CreateCommand(_mockServiceProvider.Object, _mockConsole.Object);
        var rootCommand = new RootCommand();
        rootCommand.Add(createCommand);

        // Act
        var parseResult = rootCommand.Parse($"create --pizza margherita --size {size}");

        // Assert
        Assert.NotNull(parseResult);
        Assert.Empty(parseResult.Errors);
    }

    /// <summary>
    /// Invalid size values produce errors.
    /// </summary>
    [Fact]
    public void Parse_CreateCommandWithInvalidSize_ReturnsError()
    {
        // Arrange
        var createCommand = new CreateCommand(_mockServiceProvider.Object, _mockConsole.Object);
        var rootCommand = new RootCommand();
        rootCommand.Add(createCommand);

        // Act
        var parseResult = rootCommand.Parse("create --pizza margherita --size extralarge");

        // Assert
        Assert.NotNull(parseResult);
        Assert.NotEmpty(parseResult.Errors);
    }

    /// <summary>
    /// Ensures pizza validation is case-insensitive.
    /// </summary>
    [Theory]
    [InlineData("margherita")]
    [InlineData("MARGHERITA")]
    [InlineData("Margherita")]
    [InlineData("MaRgHeRiTa")]
    public void Parse_CreateCommandWithDifferentCasing_ParsesCorrectly(string pizzaName)
    {
        // Arrange
        var createCommand = new CreateCommand(_mockServiceProvider.Object, _mockConsole.Object);
        var rootCommand = new RootCommand();
        rootCommand.Add(createCommand);

        // Act
        var parseResult = rootCommand.Parse($"create --pizza {pizzaName}");

        // Assert
        Assert.NotNull(parseResult);
        Assert.Empty(parseResult.Errors);
    }

    /// <summary>
    /// Parses comma-separated toppings successfully.
    /// </summary>
    [Fact]
    public void Parse_CreateCommandWithCommaSeparatedToppings_ParsesCorrectly()
    {
        // Arrange
        var createCommand = new CreateCommand(_mockServiceProvider.Object, _mockConsole.Object);
        var rootCommand = new RootCommand();
        rootCommand.Add(createCommand);

        // Act
        var parseResult = rootCommand.Parse("create --pizza margherita --toppings basil,mozzarella,olive");

        // Assert
        Assert.NotNull(parseResult);
        Assert.Empty(parseResult.Errors);
    }

    /// <summary>
    /// Parses multiple `--toppings` arguments successfully.
    /// </summary>
    [Fact]
    public void Parse_CreateCommandWithMultipleToppingsArguments_ParsesCorrectly()
    {
        // Arrange
        var createCommand = new CreateCommand(_mockServiceProvider.Object, _mockConsole.Object);
        var rootCommand = new RootCommand();
        rootCommand.Add(createCommand);

        // Act
        var parseResult = rootCommand.Parse("create --pizza margherita --toppings basil --toppings mozzarella");

        // Assert
        Assert.NotNull(parseResult);
        Assert.Empty(parseResult.Errors);
    }

    /// <summary>
    /// Parses supported output formats successfully.
    /// </summary>
    [Theory]
    [InlineData("table")]
    [InlineData("json")]
    [InlineData("yaml")]
    public void Parse_CreateCommandWithDifferentOutputFormats_ParsesCorrectly(string outputFormat)
    {
        // Arrange
        var createCommand = new CreateCommand(_mockServiceProvider.Object, _mockConsole.Object);
        var rootCommand = new RootCommand();
        rootCommand.Add(createCommand);

        // Act
        var parseResult = rootCommand.Parse($"create --pizza margherita --output {outputFormat}");

        // Assert
        Assert.NotNull(parseResult);
        Assert.Empty(parseResult.Errors);
    }

    /// <summary>
    /// Invalid output formats produce errors.
    /// </summary>
    [Fact]
    public void Parse_CreateCommandWithInvalidOutputFormat_ReturnsError()
    {
        // Arrange
        var createCommand = new CreateCommand(_mockServiceProvider.Object, _mockConsole.Object);
        var rootCommand = new RootCommand();
        rootCommand.Add(createCommand);

        // Act
        var parseResult = rootCommand.Parse("create --pizza margherita --output invalid");

        // Assert
        Assert.NotNull(parseResult);
        Assert.NotEmpty(parseResult.Errors);
    }

    /// <summary>
    /// Parses boolean delivery values successfully.
    /// </summary>
    [Theory]
    [InlineData("true")]
    [InlineData("false")]
    public void Parse_CreateCommandWithDeliveryOption_ParsesCorrectly(string deliveryValue)
    {
        // Arrange
        var createCommand = new CreateCommand(_mockServiceProvider.Object, _mockConsole.Object);
        var rootCommand = new RootCommand();
        rootCommand.Add(createCommand);

        // Act
        var parseResult = rootCommand.Parse($"create --pizza margherita --delivery {deliveryValue}");

        // Assert
        Assert.NotNull(parseResult);
        Assert.Empty(parseResult.Errors);
    }

    /// <summary>
    /// Parses from full hierarchy and verifies resolution.
    /// </summary>
    [Fact]
    public void Parse_CreateCommandInHierarchy_ParsesCorrectly()
    {
        // Arrange
        var orderCommand = new OrderCommand(_mockServiceProvider.Object, _mockConsole.Object);
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
