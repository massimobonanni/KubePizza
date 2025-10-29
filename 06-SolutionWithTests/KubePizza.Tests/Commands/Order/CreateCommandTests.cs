using KubePizza.Console.Commands.Order;
using KubePizza.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.CommandLine;

namespace KubePizza.Tests.Commands.Order;

/// <summary>
/// Unit tests for the CreateCommand class.
/// These tests focus on the core functionality and behavior of the CreateCommand,
/// including constructor validation, property verification, and option setup.
/// 
/// The CreateCommand is responsible for creating new pizza orders with various options
/// like pizza type, size, toppings, and delivery preferences. It uses dependency injection
/// to access the pizza catalog service and provides validation and tab completion features.
/// </summary>
public class CreateCommandTests
{
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly Mock<IPizzaCatalog> _mockPizzaCatalog;
    private readonly CreateCommand _createCommand;

    /// <summary>
    /// Test setup that runs before each test method.
    /// Creates mock dependencies and initializes a CreateCommand instance for testing.
    /// This ensures each test starts with a clean, isolated state.
    /// </summary>
    public CreateCommandTests()
    {
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockPizzaCatalog = new Mock<IPizzaCatalog>();

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

        _createCommand = new CreateCommand(_mockServiceProvider.Object);
    }

    /// <summary>
    /// Verifies that the CreateCommand constructor sets the correct command name.
    /// 
    /// Purpose: Ensures the command can be invoked using the correct name "create"
    /// How it works: Creates a CreateCommand and checks that its Name property equals "create"
    /// </summary>
    [Fact]
    public void Constructor_SetsCorrectName()
    {
        // Assert
        Assert.Equal("create", _createCommand.Name);
    }

    /// <summary>
    /// Verifies that the CreateCommand constructor sets the correct description.
    /// 
    /// Purpose: Ensures users see meaningful help text when using --help
    /// How it works: Creates a CreateCommand and verifies the Description property
    /// </summary>
    [Fact]
    public void Constructor_SetsCorrectDescription()
    {
        // Assert
        Assert.Equal("Create a new pizza order", _createCommand.Description);
    }

    /// <summary>
    /// Verifies that the CreateCommand has all required options configured.
    /// 
    /// Purpose: Ensures all necessary options are available for users
    /// How it works: Checks that the command has the expected options
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
    /// Verifies that the pizza option is configured as required.
    /// 
    /// Purpose: Ensures users must specify a pizza type
    /// How it works: Locates the pizza option and verifies it's marked as required
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
    /// Verifies that the size option is configured as optional with a default value.
    /// 
    /// Purpose: Ensures size has sensible defaults for user convenience
    /// How it works: Locates the size option and verifies it's not required
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
    /// Verifies that the toppings option is configured to accept multiple values.
    /// 
    /// Purpose: Ensures users can specify multiple toppings for their pizza
    /// How it works: Locates the toppings option and verifies its arity settings
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
    /// Verifies that the delivery option is configured as optional.
    /// 
    /// Purpose: Ensures delivery preference is optional with a default
    /// How it works: Locates the delivery option and verifies it's not required
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
    /// Verifies that CreateCommand properly inherits from CommandBase.
    /// 
    /// Purpose: Ensures the inheritance hierarchy is correct and CreateCommand gets base functionality
    /// How it works: Uses Assert.IsAssignableFrom to check inheritance relationship
    /// </summary>
    [Fact]
    public void Constructor_InheritsFromCommandBase()
    {
        // Assert
        Assert.IsAssignableFrom<KubePizza.Console.Commands.CommandBase>(_createCommand);
    }

    /// <summary>
    /// Verifies that CreateCommand inherits from System.CommandLine.Command.
    /// 
    /// Purpose: Ensures CreateCommand is compatible with System.CommandLine framework
    /// How it works: Uses Assert.IsAssignableFrom to check inheritance from the base Command class
    /// </summary>
    [Fact]
    public void Constructor_InheritsFromCommand()
    {
        // Assert
        Assert.IsAssignableFrom<Command>(_createCommand);
    }

    /// <summary>
    /// Verifies that passing null as the service provider throws an ArgumentNullException.
    /// 
    /// Purpose: Ensures proper error handling for invalid constructor parameters
    /// How it works: Attempts to create a CreateCommand with null service provider
    /// and verifies that ArgumentNullException is thrown
    /// </summary>
    [Fact]
    public void Constructor_WithNullServiceProvider_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CreateCommand(null!));
    }

    /// <summary>
    /// Verifies that CreateCommand inherits the --output option from CommandBase.
    /// 
    /// Purpose: Ensures the base functionality for output formatting is available
    /// How it works: Searches the Options collection for the --output option or its -o alias
    /// </summary>
    [Fact]
    public void CreateCommand_InheritsOutputOptionFromCommandBase()
    {
        // Assert
        Assert.Contains(_createCommand.Options, opt => opt.Name == "--output" || opt.Aliases.Contains("-o"));
    }

    /// <summary>
    /// Verifies that the pizza option has validators configured.
    /// 
    /// Purpose: Ensures pizza type validation is set up
    /// How it works: Locates the pizza option and checks that it has validators
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
    /// Verifies that the pizza option has completion sources configured.
    /// 
    /// Purpose: Ensures tab completion is available for pizza types
    /// How it works: Locates the pizza option and checks that it has completion sources
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
    /// Verifies that the size option has completion sources configured.
    /// 
    /// Purpose: Ensures tab completion is available for pizza sizes
    /// How it works: Locates the size option and checks that it has completion sources
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
    /// Verifies that the toppings option has completion sources configured.
    /// 
    /// Purpose: Ensures tab completion is available for toppings
    /// How it works: Locates the toppings option and checks that it has completion sources
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
    /// Verifies that the command can be invoked with valid pizza type.
    /// 
    /// Purpose: Tests basic command parsing functionality
    /// How it works: Creates a root command, adds the create command, and parses valid input
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
    /// Verifies that the command generates errors when required pizza is missing.
    /// 
    /// Purpose: Tests validation for required parameters
    /// How it works: Attempts to parse a command without the required pizza option
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
    /// Verifies that pizza option descriptions are user-friendly.
    /// 
    /// Purpose: Ensures good user experience with helpful option descriptions
    /// How it works: Checks that the pizza option has a meaningful description
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
    /// Verifies that toppings option has a clear description.
    /// 
    /// Purpose: Ensures users understand how to specify toppings
    /// How it works: Checks that the toppings option has a descriptive text
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
    /// Verifies that delivery option has a clear description with default information.
    /// 
    /// Purpose: Ensures users understand the delivery option and its default
    /// How it works: Checks that the delivery option has informative description text
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