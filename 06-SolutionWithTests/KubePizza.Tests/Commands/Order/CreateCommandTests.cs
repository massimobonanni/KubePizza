using KubePizza.Console.Commands.Order;
using KubePizza.Core.Interfaces;
using KubePizza.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.CommandLine;

namespace KubePizza.Tests.Commands.Order;

/// <summary>
/// Unit tests for <see cref="CreateCommand"/> covering constructor validation, properties,
/// options, completion, validators, and basic parsing behavior.
/// </summary>
public class CreateCommandTests
{
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly Mock<IPizzaCatalog> _mockPizzaCatalog;
    private readonly Mock<IConsole> _mockConsole;
    private readonly CreateCommand _createCommand;

    /// <summary>
    /// Initializes common mocks and the command instance used across tests.
    /// </summary>
    public CreateCommandTests()
    {
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockPizzaCatalog = new Mock<IPizzaCatalog>();
        _mockConsole = new Mock<IConsole>();

        // Setup mock pizza catalog with test data
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

        _createCommand = new CreateCommand(_mockServiceProvider.Object, _mockConsole.Object);
    }

    /// <summary>
    /// Ensures the command name is set to "create".
    /// </summary>
    [Fact]
    public void Constructor_SetsCorrectName()
    {
        // Assert
        Assert.Equal("create", _createCommand.Name);
    }

    /// <summary>
    /// Ensures the description is user friendly.
    /// </summary>
    [Fact]
    public void Constructor_SetsCorrectDescription()
    {
        // Assert
        Assert.Equal("Create a new pizza order", _createCommand.Description);
    }

    /// <summary>
    /// Ensures required options exist.
    /// </summary>
    [Fact]
    public void Constructor_AddsAllRequiredOptions()
    {
        // Arrange
        var optionNames = _createCommand.Options.Select(o => o.Name).ToList();

        // Assert
        Assert.Contains("--pizza", optionNames);
        Assert.Contains("--size", optionNames);
        Assert.Contains("--toppings", optionNames);
        Assert.Contains("--delivery", optionNames);
    }

    /// <summary>
    /// Ensures `--pizza` is required.
    /// </summary>
    [Fact]
    public void Constructor_PizzaOptionIsRequired()
    {
        // Arrange
        var pizzaOption = _createCommand.Options.FirstOrDefault(o => o.Name == "--pizza");

        // Assert
        Assert.NotNull(pizzaOption);
        Assert.True(pizzaOption.Required);
    }

    /// <summary>
    /// Ensures `--size` is optional.
    /// </summary>
    [Fact]
    public void Constructor_SizeOptionIsOptional()
    {
        // Arrange
        var sizeOption = _createCommand.Options.FirstOrDefault(o => o.Name == "--size");

        // Assert
        Assert.NotNull(sizeOption);
        Assert.False(sizeOption.Required);
    }

    /// <summary>
    /// Ensures `--toppings` accepts multiple values.
    /// </summary>
    [Fact]
    public void Constructor_ToppingsOptionAcceptsMultipleValues()
    {
        // Arrange
        var toppingsOption = _createCommand.Options.FirstOrDefault(o => o.Name == "--toppings");

        // Assert
        Assert.NotNull(toppingsOption);
        Assert.Equal(ArgumentArity.ZeroOrMore, toppingsOption.Arity);
        Assert.True(toppingsOption.AllowMultipleArgumentsPerToken);
    }

    /// <summary>
    /// Ensures `--delivery` is optional.
    /// </summary>
    [Fact]
    public void Constructor_DeliveryOptionIsOptional()
    {
        // Arrange
        var deliveryOption = _createCommand.Options.FirstOrDefault(o => o.Name == "--delivery");

        // Assert
        Assert.NotNull(deliveryOption);
        Assert.False(deliveryOption.Required);
    }

    /// <summary>
    /// Confirms inheritance from `CommandBase`.
    /// </summary>
    [Fact]
    public void Constructor_InheritsFromCommandBase()
    {
        // Assert
        Assert.IsAssignableFrom<KubePizza.Console.Commands.CommandBase>(_createCommand);
    }

    /// <summary>
    /// Confirms inheritance from `System.CommandLine.Command`.
    /// </summary>
    [Fact]
    public void Constructor_InheritsFromCommand()
    {
        // Assert
        Assert.IsAssignableFrom<Command>(_createCommand);
    }

    /// <summary>
    /// Validates null service provider throws `ArgumentNullException`.
    /// </summary>
    [Fact]
    public void Constructor_WithNullServiceProvider_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CreateCommand(null!, _mockConsole.Object));
    }

    /// <summary>
    /// Confirms inherited `--output` option exists.
    /// </summary>
    [Fact]
    public void CreateCommand_InheritsOutputOptionFromCommandBase()
    {
        // Assert
        Assert.Contains(_createCommand.Options, opt => opt.Name == "--output" || opt.Aliases.Contains("-o"));
    }

    /// <summary>
    /// Ensures `--pizza` has validators.
    /// </summary>
    [Fact]
    public void Constructor_PizzaOptionHasValidators()
    {
        // Arrange
        var pizzaOption = _createCommand.Options.FirstOrDefault(o => o.Name == "--pizza");

        // Assert
        Assert.NotNull(pizzaOption);
        Assert.NotEmpty(pizzaOption.Validators);
    }

    /// <summary>
    /// Ensures `--pizza` has completion sources.
    /// </summary>
    [Fact]
    public void Constructor_PizzaOptionHasCompletionSources()
    {
        // Arrange
        var pizzaOption = _createCommand.Options.FirstOrDefault(o => o.Name == "--pizza");

        // Assert
        Assert.NotNull(pizzaOption);
        Assert.NotEmpty(pizzaOption.CompletionSources);
    }

    /// <summary>
    /// Ensures `--size` has completion sources.
    /// </summary>
    [Fact]
    public void Constructor_SizeOptionHasCompletionSources()
    {
        // Arrange
        var sizeOption = _createCommand.Options.FirstOrDefault(o => o.Name == "--size");

        // Assert
        Assert.NotNull(sizeOption);
        Assert.NotEmpty(sizeOption.CompletionSources);
    }

    /// <summary>
    /// Ensures `--toppings` has completion sources.
    /// </summary>
    [Fact]
    public void Constructor_ToppingsOptionHasCompletionSources()
    {
        // Arrange
        var toppingsOption = _createCommand.Options.FirstOrDefault(o => o.Name == "--toppings");

        // Assert
        Assert.NotNull(toppingsOption);
        Assert.NotEmpty(toppingsOption.CompletionSources);
    }

    /// <summary>
    /// Parses valid input successfully.
    /// </summary>
    [Fact]
    public void Command_WithValidPizza_ParsesSuccessfully()
    {
        // Arrange
        var rootCommand = new RootCommand();
        rootCommand.Add(_createCommand);

        // Act
        var parseResult = rootCommand.Parse("create --pizza margherita");

        // Assert
        Assert.Empty(parseResult.Errors);
    }

    /// <summary>
    /// Missing required `--pizza` generates errors.
    /// </summary>
    [Fact]
    public void Command_WithoutRequiredPizza_GeneratesError()
    {
        // Arrange
        var rootCommand = new RootCommand();
        rootCommand.Add(_createCommand);

        // Act
        var parseResult = rootCommand.Parse("create --size large");

        // Assert
        Assert.NotEmpty(parseResult.Errors);
    }

    /// <summary>
    /// Ensures `--pizza` has a user-friendly description.
    /// </summary>
    [Fact]
    public void Constructor_PizzaOptionHasUserFriendlyDescription()
    {
        // Arrange
        var pizzaOption = _createCommand.Options.FirstOrDefault(o => o.Name == "--pizza");

        // Assert
        Assert.NotNull(pizzaOption);
        Assert.Equal("Type of pizza (e.g. margherita, diavola)", pizzaOption.Description);
    }

    /// <summary>
    /// Ensures `--toppings` has a clear description.
    /// </summary>
    [Fact]
    public void Constructor_ToppingsOptionHasUserFriendlyDescription()
    {
        // Arrange
        var toppingsOption = _createCommand.Options.FirstOrDefault(o => o.Name == "--toppings");

        // Assert
        Assert.NotNull(toppingsOption);
        Assert.Equal("List of extra toppings (comma-separated).", toppingsOption.Description);
    }

    /// <summary>
    /// Ensures `--delivery` has a clear description with default info.
    /// </summary>
    [Fact]
    public void Constructor_DeliveryOptionHasUserFriendlyDescription()
    {
        // Arrange
        var deliveryOption = _createCommand.Options.FirstOrDefault(o => o.Name == "--delivery");

        // Assert
        Assert.NotNull(deliveryOption);
        Assert.Equal("Specify if the order is for delivery (default: true).", deliveryOption.Description);
    }
}