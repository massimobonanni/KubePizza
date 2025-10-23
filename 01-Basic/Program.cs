/// <summary>
/// KubePizza command-line application entry point.
/// This application provides a command-line interface for managing pizza orders with various options
/// including pizza type, size, toppings, and delivery preferences.
/// </summary>

using KubePizza.Core.Utilities;
using System.CommandLine;

ConsoleUtility.WriteApplicationBanner();

/// <summary>
/// Define the root command for the kubepizza application.
/// This command serves as the main entry point for pizza order management.
/// </summary>
var root = new RootCommand("kubepizza — manage your pizza orders");

/// <summary>
/// Define command options for pizza ordering.
/// </summary>

/// <summary>
/// Pizza type option - specifies the type of pizza to order.
/// This is a required parameter that accepts pizza names like "margherita" or "diavola".
/// </summary>
var pizzaOption = new Option<string>("--pizza")
{
    Description = "The type of pizza to order (e.g. margherita, diavola).",
    Required = true,
};

/// <summary>
/// Pizza size option - specifies the size of the pizza.
/// Accepts only predefined values: small, medium, large.
/// Defaults to "medium" if not specified.
/// </summary>
var sizeOption = new Option<string>("--size")
{
    Description = "The size of the pizza: small, medium, large.",
};
sizeOption.Aliases.Add("-s");
sizeOption.DefaultValueFactory = (argResult) => "medium";
sizeOption.AcceptOnlyFromAmong("small", "medium", "large");

/// <summary>
/// Toppings option - specifies additional toppings for the pizza.
/// Accepts zero or more toppings as a comma-separated list or multiple arguments.
/// </summary>
var toppingsOption = new Option<string[]>("--toppings")
{
    Description = "Comma-separated list of extra toppings.",
    Arity = ArgumentArity.ZeroOrMore
};
toppingsOption.AllowMultipleArgumentsPerToken = true;

/// <summary>
/// Delivery option - specifies whether the order is for delivery or pickup.
/// Boolean flag that determines the fulfillment method for the order.
/// </summary>
var deliveryOption = new Option<bool>("--delivery")
{
    Description = "Whether the order is for delivery (true) or pickup (false)."
};

/// <summary>
/// Add all command options to the root command.
/// </summary>
root.Options.Add(pizzaOption);
root.Options.Add(sizeOption);
root.Options.Add(toppingsOption);
root.Options.Add(deliveryOption);

/// <summary>
/// Set the command handler that processes the parsed command-line arguments.
/// This handler extracts option values and displays the order information.
/// In a production application, this would integrate with business services.
/// </summary>
root.SetAction((ParseResult parseResult) =>
{
    /// <summary>
    /// Retrieve and validate option values from the parsed command-line arguments.
    /// </summary>
    var pizza = parseResult.GetRequiredValue(pizzaOption);
    var size = parseResult.GetValue(sizeOption);
    var toppings = parseResult.GetValue(toppingsOption);
    var delivery = parseResult.GetValue(deliveryOption);

    /// <summary>
    /// Display the processed order information to the console.
    /// In a real application, this data would be passed to order processing services.
    /// </summary>
    ConsoleUtility.WriteLine($"Order received:",ConsoleColor.Green);
    ConsoleUtility.WriteLine($"\t- Pizza: {pizza}");
    ConsoleUtility.WriteLine($"\t- Size: {size}");
    ConsoleUtility.WriteLine($"\t- Toppings: {(toppings != null && toppings.Length > 0 ? string.Join(", ", toppings) : "(none)")}");
    ConsoleUtility.WriteLine($"\t- Delivery: {delivery}");
});

/// <summary>
/// Parse the command-line arguments and invoke the appropriate handler.
/// Returns the exit code from the command execution.
/// </summary>
ParseResult parseResult = root.Parse(args);
return parseResult.Invoke();