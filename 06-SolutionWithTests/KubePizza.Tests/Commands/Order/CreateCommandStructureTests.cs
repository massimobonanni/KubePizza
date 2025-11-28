using KubePizza.Console.Commands.Order;
using KubePizza.Core.Interfaces;
using KubePizza.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.CommandLine;

namespace KubePizza.Tests.Commands.Order;

/// <summary>
/// Structure tests for <see cref="CreateCommand"/> covering properties, options, types,
/// inheritance, aliases, completion, and arity configuration.
/// </summary>
public class CreateCommandStructureTests
{
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly Mock<IPizzaCatalog> _mockPizzaCatalog;
    private readonly Mock<IConsole> _mockConsole;
    private readonly CreateCommand _createCommand;

    /// <summary>
    /// Initializes common mocks and command instance for structure tests.
    /// </summary>
    public CreateCommandStructureTests()
    {
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockPizzaCatalog = new Mock<IPizzaCatalog>();
        _mockConsole = new Mock<IConsole>();

        // Setup standard test data
        _mockPizzaCatalog.Setup(pc => pc.Pizzas)
          .Returns(new[] { "margherita", "diavola", "capricciosa" });

        _mockPizzaCatalog.Setup(pc => pc.AllToppings)
            .Returns(new[] { "basil", "mozzarella", "olive" });

        _mockServiceProvider.Setup(sp => sp.GetService(typeof(IPizzaCatalog)))
        .Returns(_mockPizzaCatalog.Object);

        _createCommand = new CreateCommand(_mockServiceProvider.Object, _mockConsole.Object);
    }

    /// <summary>
    /// Ensures name and description are correctly configured.
    /// </summary>
    [Fact]
    public void CreateCommand_HasCorrectCommandProperties()
    {
        // Assert
        Assert.Equal("create", _createCommand.Name);
        Assert.Equal("Create a new pizza order", _createCommand.Description);
    }

    /// <summary>
    /// Ensures the command exposes the expected number of options.
    /// </summary>
    [Fact]
    public void CreateCommand_HasExpectedNumberOfOptions()
    {
        // Assert
        Assert.Equal(5, _createCommand.Options.Count);
    }

    /// <summary>
    /// Ensures expected option names exist.
    /// </summary>
    [Fact]
    public void CreateCommand_OptionsHaveCorrectNames()
    {
        // Arrange
        var optionNames = _createCommand.Options.Select(o => o.Name).ToList();

        // Assert
        Assert.Contains("--pizza", optionNames);
        Assert.Contains("--size", optionNames);
        Assert.Contains("--toppings", optionNames);
        Assert.Contains("--delivery", optionNames);
        Assert.Contains("--output", optionNames);
    }

    /// <summary>
    /// Ensures options have correct generic types.
    /// </summary>
    [Fact]
    public void CreateCommand_OptionsHaveCorrectTypes()
    {
        // Arrange & Assert
        var pizzaOption = _createCommand.Options.FirstOrDefault(o => o.Name == "--pizza");
        Assert.IsType<Option<string>>(pizzaOption);

        var sizeOption = _createCommand.Options.FirstOrDefault(o => o.Name == "--size");
        Assert.IsType<Option<string>>(sizeOption);

        var toppingsOption = _createCommand.Options.FirstOrDefault(o => o.Name == "--toppings");
        Assert.IsType<Option<string[]>>(toppingsOption);

        var deliveryOption = _createCommand.Options.FirstOrDefault(o => o.Name == "--delivery");
        Assert.IsType<Option<bool>>(deliveryOption);

        var outputOption = _createCommand.Options.FirstOrDefault(o => o.Name == "--output");
        Assert.IsType<Option<string>>(outputOption);
    }

    /// <summary>
    /// Confirms inheritance from `CommandBase`.
    /// </summary>
    [Fact]
    public void CreateCommand_InheritsFromCommandBase()
    {
        // Assert
        Assert.IsAssignableFrom<KubePizza.Console.Commands.CommandBase>(_createCommand);
    }

    /// <summary>
    /// Confirms it is assignable to `System.CommandLine.Command`.
    /// </summary>
    [Fact]
    public void CreateCommand_ImplementsSystemCommandLineCommand()
    {
        // Assert
        Assert.IsAssignableFrom<Command>(_createCommand);
    }

    /// <summary>
    /// Ensures inherited `--output` option exists with alias `-o`.
    /// </summary>
    [Fact]
    public void CreateCommand_HasOutputOptionFromBase()
    {
        // Arrange
        var outputOption = _createCommand.Options.FirstOrDefault(o => o.Name == "--output");

        // Assert
        Assert.NotNull(outputOption);
        Assert.Contains("-o", outputOption.Aliases);
    }

    /// <summary>
    /// Verifies `Required` configuration for multiple options.
    /// </summary>
    [Theory]
    [InlineData("--pizza", true)]
    [InlineData("--size", false)]
    [InlineData("--toppings", false)]
    [InlineData("--delivery", false)]
    [InlineData("--output", false)]
    public void CreateCommand_OptionHasCorrectRequiredState(string optionName, bool expectedRequired)
    {
        // Arrange
        var option = _createCommand.Options.FirstOrDefault(o => o.Name == optionName);

        // Assert
        Assert.NotNull(option);
        Assert.Equal(expectedRequired, option.Required);
    }

    /// <summary>
    /// Ensures it can be added under `OrderCommand`.
    /// </summary>
    [Fact]
    public void CreateCommand_CanBeAddedToParentCommand()
    {
        // Arrange
        var orderCommand = new OrderCommand(_mockServiceProvider.Object, _mockConsole.Object);

        // Assert
        Assert.Contains(orderCommand.Subcommands, cmd => cmd.Name == "create");
    }

    /// <summary>
    /// Ensures every option exposes a non-empty description.
    /// </summary>
    [Fact]
    public void CreateCommand_AllOptionsHaveDescriptions()
    {
        // Assert
        foreach (var option in _createCommand.Options)
        {
            Assert.NotNull(option.Description);
            Assert.NotEmpty(option.Description);
        }
    }

    /// <summary>
    /// Ensures the pizza option has validators and completion sources.
    /// </summary>
    [Fact]
    public void CreateCommand_PizzaOptionHasValidationAndCompletion()
    {
        // Arrange
        var pizzaOption = _createCommand.Options.FirstOrDefault(o => o.Name == "--pizza");

        // Assert
        Assert.NotNull(pizzaOption);
        Assert.NotEmpty(pizzaOption.Validators);
        Assert.NotEmpty(pizzaOption.CompletionSources);
    }

    /// <summary>
    /// Ensures options expose completion sources.
    /// </summary>
    [Theory]
    [InlineData("--pizza")]
    [InlineData("--size")]
    [InlineData("--toppings")]
    public void CreateCommand_OptionHasCompletionSources(string optionName)
    {
        // Arrange
        var option = _createCommand.Options.FirstOrDefault(o => o.Name == optionName);

        // Assert
        Assert.NotNull(option);
        Assert.NotEmpty(option.CompletionSources);
    }

    /// <summary>
    /// Confirms toppings option allows multiple arguments and has correct arity.
    /// </summary>
    [Fact]
    public void CreateCommand_ToppingsOptionAllowsMultipleArguments()
    {
        // Arrange
        var toppingsOption = _createCommand.Options.FirstOrDefault(o => o.Name == "--toppings");

        // Assert
        Assert.NotNull(toppingsOption);
        Assert.Equal(ArgumentArity.ZeroOrMore, toppingsOption.Arity);
        Assert.True(toppingsOption.AllowMultipleArgumentsPerToken);
    }

    /// <summary>
    /// Ensures no positional arguments are defined.
    /// </summary>
    [Fact]
    public void CreateCommand_HasNoArguments()
    {
        // Assert
        Assert.Empty(_createCommand.Arguments);
    }

    /// <summary>
    /// Confirms the command is configured with a handler (indirectly via parsing).
    /// </summary>
    [Fact]
    public void CreateCommand_HasHandlerConfigured()
    {
        // Arrange
        var rootCommand = new RootCommand();
        rootCommand.Add(_createCommand);

        // Act
        var parseResult = rootCommand.Parse("create --pizza margherita");

        // Assert
        Assert.Empty(parseResult.Errors);
        Assert.Equal("create", parseResult.CommandResult.Command.Name);
    }

    /// <summary>
    /// Ensures the output option exposes alias `-o`.
    /// </summary>
    [Fact]
    public void CreateCommand_OutputOptionHasAlias()
    {
        // Arrange
        var outputOption = _createCommand.Options.FirstOrDefault(o => o.Name == "--output");

        // Assert
        Assert.NotNull(outputOption);
        Assert.Contains("-o", outputOption.Aliases);
        Assert.Single(outputOption.Aliases);
    }
}
