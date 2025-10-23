using _03_Validators.Commands;
using KubePizza.Core.Services;
using KubePizza.Core.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Drawing;
using System.Text;

namespace _03_Validators.Commands.Order;

internal class CreateCommand : CommandBase
{
    private readonly Option<string> pizzaOption;
    private readonly Option<string> sizeOption;
    private readonly Option<string[]> toppingsOption;
    private readonly Option<bool> deliveryOption;

    public CreateCommand(IServiceProvider serviceProvider) : base("create", "Create a new pizza order", serviceProvider)
    {
        pizzaOption = new Option<string>("--pizza")
        {
            Description = "Type of pizza (e.g. margherita, diavola)"
        };
        pizzaOption.Required = true;

        // Pizza must be in the catalog (validator for an option)
        pizzaOption.Validators.Add(result =>
        {
            var pizzaCatalog = this.serviceProvider.GetRequiredService<IPizzaCatalog>();
            var value = result.GetValueOrDefault<string>();
            if (!pizzaCatalog.Pizzas.Contains(value, StringComparer.OrdinalIgnoreCase))
            {
                result.AddError($"Invalid pizza type '{value}'. Allowed types are: {string.Join(", ", pizzaCatalog.Pizzas)}.");
            }
        });

        sizeOption = new Option<string>("--size")
        {
            DefaultValueFactory = (a) => "medium",
        };
        sizeOption.AcceptOnlyFromAmong("small", "medium", "large");

        toppingsOption = new Option<string[]>("--toppings")
        {
            Description = "List of extra toppings (comma-separated).",
            Arity = ArgumentArity.ZeroOrMore,
            AllowMultipleArgumentsPerToken = true
        };

        // topping must be valid (if provided) (validator for an option)
        toppingsOption.Validators.Add(result =>
        {
            var pizzaCatalog = this.serviceProvider.GetRequiredService<IPizzaCatalog>();
            var values = result.GetValueOrDefault<string[]>() ?? Array.Empty<string>();
            var invalidToppings = values
                .Where(t => !pizzaCatalog.AllToppings.Contains(t, StringComparer.OrdinalIgnoreCase))
                .ToArray();
            if (invalidToppings.Length > 0)
            {
                result.AddError($"Invalid toppings: {string.Join(", ", invalidToppings)}. Allowed toppings are: {string.Join(", ", pizzaCatalog.AllToppings)}.");
            }
        });

        deliveryOption = new Option<bool>("--delivery")
        {
            Description = "Specify if the order is for delivery (default: true).",
            DefaultValueFactory = _ => true,
        };

        // Validate options values in combination (validator for the command)
        this.Validators.Add(result =>
        {
            var size = result.GetValue(sizeOption) ?? "medium";
            var toppings = result.GetValue(toppingsOption) ?? Array.Empty<string>();

            // If size=small and toppings > 3 → error “business”
            if (size.Equals("small", StringComparison.OrdinalIgnoreCase) && toppings.Length > 3)
            {
                result.AddError("Too many toppings for a small size (max 3).");
            }
        });

        this.Options.Add(pizzaOption);
        this.Options.Add(sizeOption);
        this.Options.Add(toppingsOption);
        this.Options.Add(deliveryOption);

        this.SetAction(CommandHandler);
    }

    private async Task CommandHandler(ParseResult parseResult, CancellationToken cancellationToken)
    {
        var pizza = parseResult.GetRequiredValue(pizzaOption);
        var size = parseResult.GetValue(sizeOption);
        var toppings = parseResult.GetValue(toppingsOption);
        var delivery = parseResult.GetValue(deliveryOption);
        var output = parseResult.GetValue<string>(outputOption);

        ConsoleUtility.WriteLine($"🍕 Creating order:", ConsoleColor.Green);

        await Task.Delay(5000, cancellationToken)
            .WithLoadingIndicator(
                    message: $"Sendig order to server...",
                    style: LoadingIndicator.Style.Spinner,
                    completionMessage: $"Order placed successfully!",
                    showTimeTaken: true); ;

        Console.WriteLine($"\tPizza: {pizza}");
        Console.WriteLine($"\tSize: {size}");
        Console.WriteLine($"\tToppings: {(toppings.Length > 0 ? string.Join(", ", toppings) : "(none)")}");
        Console.WriteLine($"\tDelivery: {delivery}");
        Console.WriteLine($"\tOutput format: {output}");
    }
}
