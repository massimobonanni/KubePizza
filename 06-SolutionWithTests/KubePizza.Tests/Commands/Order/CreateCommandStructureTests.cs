using KubePizza.Console.Commands.Order;
using KubePizza.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.CommandLine;

namespace KubePizza.Tests.Commands.Order;

/// <summary>
/// Structure tests for the CreateCommand class.
/// These tests focus on validating the structural aspects and architectural compliance
/// of the CreateCommand, including inheritance relationships, option configuration,
/// and the proper setup of command properties.
/// 
/// Structure tests are important for maintaining code quality and ensuring that
/// the command follows established patterns and conventions throughout the application.
/// They help catch architectural violations and ensure consistency across the codebase.
/// </summary>
public class CreateCommandStructureTests
{
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly Mock<IPizzaCatalog> _mockPizzaCatalog;
    private readonly CreateCommand _createCommand;

    /// <summary>
    /// Test setup that runs before each test method.
    /// Creates mock dependencies and initializes a CreateCommand for structure testing.
    /// This ensures consistent setup across all structure validation tests.
    /// </summary>
    public CreateCommandStructureTests()
    {
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockPizzaCatalog = new Mock<IPizzaCatalog>();

        // Setup standard test data
        _mockPizzaCatalog.Setup(pc => pc.Pizzas)
          .Returns(new[] { "margherita", "diavola", "capricciosa" });

        _mockPizzaCatalog.Setup(pc => pc.AllToppings)
 .Returns(new[] { "basil", "mozzarella", "olive" });

        _mockServiceProvider.Setup(sp => sp.GetService(typeof(IPizzaCatalog)))
  .Returns(_mockPizzaCatalog.Object);

        _createCommand = new CreateCommand(_mockServiceProvider.Object);
    }

    /// <summary>
    /// Verifies that CreateCommand has all the correct basic command properties set up properly.
    /// 
    /// Purpose: Ensures the command metadata is correctly configured for CLI framework
    /// How it works:
    /// 1. Validates the command name is "create" (used for invocation)
    /// 2. Validates the description is user-friendly and descriptive
    /// 3. This test ensures the command presents itself correctly to users
    /// </summary>
    [Fact]
    public void CreateCommand_HasCorrectCommandProperties()
    {
        // Assert
        Assert.Equal("create", _createCommand.Name);
        Assert.Equal("Create a new pizza order", _createCommand.Description);
    }

    /// <summary>
    /// Verifies that CreateCommand contains exactly the expected number of options.
    /// 
    /// Purpose: Ensures the command structure doesn't grow unexpectedly (regression protection)
    /// How it works:
    /// 1. Counts the total number of options
    /// 2. Verifies the count equals 5 (pizza, size, toppings, delivery, output)
    /// 3. This test helps catch accidentally added or removed options
    /// </summary>
    [Fact]
    public void CreateCommand_HasExpectedNumberOfOptions()
    {
        // Assert
        Assert.Equal(5, _createCommand.Options.Count);
    }

    /// <summary>
    /// Verifies that the options have the expected names for user invocation.
    /// 
    /// Purpose: Ensures users can invoke the correct option names
    /// How it works:
    /// 1. Extracts all option names into a list
    /// 2. Verifies that each expected option exists in the list
    /// 3. This ensures users can type the correct option names
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
    /// Verifies that options are configured with the correct types.
    /// 
    /// Purpose: Ensures type safety and proper option handling
    /// How it works:
    /// 1. Locates each option by name
    /// 2. Verifies each option has the expected generic type
    /// 3. This ensures the correct data types are used for each option
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
    /// Verifies that CreateCommand properly inherits from the CommandBase class.
    /// 
    /// Purpose: Ensures architectural compliance and inheritance of shared functionality
    /// How it works:
    /// 1. Uses reflection to verify CreateCommand is assignable from CommandBase
    /// 2. This ensures CreateCommand gets shared functionality like output options
    /// 3. This test catches inheritance chain violations
    /// </summary>
    [Fact]
    public void CreateCommand_InheritsFromCommandBase()
    {
        // Assert
        Assert.IsAssignableFrom<KubePizza.Console.Commands.CommandBase>(_createCommand);
    }

    /// <summary>
    /// Verifies that CreateCommand implements the System.CommandLine.Command interface.
    /// 
    /// Purpose: Ensures compatibility with the System.CommandLine framework
    /// How it works:
    /// 1. Verifies CreateCommand can be treated as a Command (framework requirement)
    /// 2. This ensures the command can be added to parent commands and parsed properly
    /// 3. This is a fundamental requirement for CLI framework integration
    /// </summary>
    [Fact]
    public void CreateCommand_ImplementsSystemCommandLineCommand()
    {
        // Assert
        Assert.IsAssignableFrom<Command>(_createCommand);
    }

    /// <summary>
    /// Verifies that CreateCommand inherits the --output option from CommandBase.
    /// 
    /// Purpose: Ensures users can control output formatting
    /// How it works:
    /// 1. Searches the command's options for --output option
    /// 2. Verifies the option also has the -o alias
    /// 3. This ensures consistent output formatting capabilities
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
    /// Parameterized test that verifies required vs optional options are correctly configured.
    /// 
    /// Purpose: Ensures proper distinction between required and optional parameters
    /// How it works:
    /// 1. Uses Theory/InlineData to test multiple option/required pairs
    /// 2. For each pair, locates the option by name
    /// 3. Verifies the option's Required property matches expected value
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
    /// Verifies that CreateCommand can be properly added to a parent command.
    /// 
    /// Purpose: Tests integration capability with the CLI framework's command tree
    /// How it works:
    /// 1. Creates a parent OrderCommand
    /// 2. Verifies the CreateCommand appears in the parent's subcommands collection
    /// 3. This ensures the command can be integrated into a CLI application
    /// </summary>
    [Fact]
    public void CreateCommand_CanBeAddedToParentCommand()
    {
        // Arrange
        var orderCommand = new OrderCommand(_mockServiceProvider.Object);

        // Assert
        Assert.Contains(orderCommand.Subcommands, cmd => cmd.Name == "create");
    }

    /// <summary>
    /// Verifies that all options have meaningful descriptions for user help.
    /// 
    /// Purpose: Ensures good user experience with helpful option descriptions
    /// How it works:
    /// 1. Iterates through all command options
    /// 2. For each option, verifies it has a non-null, non-empty description
    /// 3. This ensures users can understand what each option does
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
    /// Verifies that the pizza option has both validators and completion sources.
    /// 
    /// Purpose: Ensures pizza option has comprehensive input assistance
    /// How it works:
    /// 1. Locates the pizza option
    /// 2. Verifies it has validators for input validation
    /// 3. Verifies it has completion sources for tab completion
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
    /// Verifies that options with completion have at least one completion source.
    /// 
    /// Purpose: Ensures tab completion is properly configured for applicable options
    /// How it works:
    /// 1. Tests specific options that should have completion sources
    /// 2. Verifies each has at least one completion source configured
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
    /// Verifies that the toppings option is configured to accept multiple arguments.
    /// 
    /// Purpose: Tests that array options have correct arity settings
    /// How it works:
    /// 1. Locates the toppings option
    /// 2. Verifies arity is set to ZeroOrMore
    /// 3. Verifies AllowMultipleArgumentsPerToken is enabled
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
    /// Verifies that CreateCommand doesn't define any command-line arguments.
    /// 
    /// Purpose: Ensures CreateCommand uses only options, not positional arguments
    /// How it works:
    /// 1. Checks the Arguments collection of CreateCommand
    /// 2. Verifies it's empty
    /// 3. This ensures all user input comes through named options
    /// </summary>
    [Fact]
    public void CreateCommand_HasNoArguments()
    {
        // Assert
        Assert.Empty(_createCommand.Arguments);
    }

    /// <summary>
    /// Verifies that the command has a handler set up for execution.
    /// 
    /// Purpose: Ensures the command can actually execute when invoked
    /// How it works:
    /// 1. Verifies the command has been configured with an action handler
    /// 2. This is tested indirectly by parsing and checking for execution capability
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
    /// Verifies that option aliases are properly configured where applicable.
    /// 
    /// Purpose: Tests that the output option has its expected alias
    /// How it works:
    /// 1. Locates the output option
    /// 2. Verifies it has the -o alias for convenience
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
