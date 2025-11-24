using KubePizza.Console.Commands;
using KubePizza.Core.Interfaces;
using KubePizza.Core.Services;
using KubePizza.Core.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Completions;
using System.Drawing;
using System.Text;

namespace KubePizza.Console.Commands.Order;

internal class CreateCommand : CommandBase
{
    private readonly Option<string> pizzaOption;
    private readonly Option<string> sizeOption;
    private readonly Option<string[]> toppingsOption;
    private readonly Option<bool> deliveryOption;

    public CreateCommand(IServiceProvider serviceProvider, IConsole console) :
        base("create", "Create a new pizza order", serviceProvider, console)
    {
        pizzaOption = new Option<string>("--pizza")
        {
            Description = "Type of pizza (e.g. margherita, diavola)"
        };
        pizzaOption.Required = true;

        // Pizza must be in the catalog (validator for an option)
        pizzaOption.Validators.Add(result =>
        {
            var pizzaCatalog = this.serviceProvider.GetPizzaCatalog();
            var value = result.GetValueOrDefault<string>();
            if (!pizzaCatalog.Pizzas.Contains(value, StringComparer.OrdinalIgnoreCase))
            {
                result.AddError($"Invalid pizza type '{value}'. Allowed types are: {string.Join(", ", pizzaCatalog.Pizzas)}.");
            }
        });

        // Dynamic completion for pizza types
        pizzaOption.CompletionSources.Add((context) =>
        {
            var pizzaCatalog = this.serviceProvider.GetPizzaCatalog();
            return pizzaCatalog.Pizzas
                .Where(p => p.Contains(context.WordToComplete, StringComparison.OrdinalIgnoreCase))
                .Select(p => new CompletionItem(p));
        });


        sizeOption = new Option<string>("--size")
        {
            Description = "Size of the pizza (small, medium, or large).",
            DefaultValueFactory = (a) => "medium",
        };
        sizeOption.AcceptOnlyFromAmong("small", "medium", "large");

        sizeOption.CompletionSources.Add((context) =>
        {
            var sizes = new[] { "small", "medium", "large" };
            return sizes
                .Where(s => s.Contains(context.WordToComplete, StringComparison.OrdinalIgnoreCase))
                .Select(s => new CompletionItem(s));
        });

        toppingsOption = new Option<string[]>("--toppings")
        {
            Description = "List of extra toppings (comma-separated).",
            Arity = ArgumentArity.ZeroOrMore,
            AllowMultipleArgumentsPerToken = true
        };

        // topping must be valid (if provided) (validator for an option)
        toppingsOption.CompletionSources.Add((context) =>
        {
            var parse = context.ParseResult;

            var chosenPizza = parse.GetValue(pizzaOption);
            var alreadyTyped = parse.GetValue(toppingsOption) ?? Array.Empty<string>();

            var pizzaCatalog = this.serviceProvider.GetPizzaCatalog();

            IEnumerable<string> toppings = string.IsNullOrWhiteSpace(chosenPizza)
                   ? pizzaCatalog.AllToppings
                   : pizzaCatalog.GetRecommendedToppingsFor(chosenPizza);

            var remaining = toppings.Except(alreadyTyped, StringComparer.OrdinalIgnoreCase);

            return remaining.Select(t => new CompletionItem(t));
        });

        toppingsOption.CustomParser = result =>
        {
            // Split by comma
            var allValues = new List<string>();
            foreach (var token in result.Tokens)
            {
                var splits = token.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                allValues.AddRange(splits);
            }
            return allValues.ToArray();
        };

        deliveryOption = new Option<bool>("--delivery")
        {
            Description = "Specify if the order is for delivery (default: true).",
            DefaultValueFactory = _ => true,
        };

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

        console.WriteLine($"Creating order:", ConsoleColor.Green);

        await Task.Delay(5000, cancellationToken)
            .WithLoadingIndicator(
                    message: $"Sendig order to server...",
                    style: LoadingIndicator.Style.Spinner,
                    completionMessage: $"Order placed successfully!",
                    showTimeTaken: true); ;

        console.WriteLine($"\tPizza: {pizza}");
        console.WriteLine($"\tSize: {size}");
        console.WriteLine($"\tToppings: {(toppings.Length > 0 ? string.Join(", ", toppings) : "(none)")}");
        console.WriteLine($"\tDelivery: {delivery}");
        console.WriteLine($"\tOutput format: {output}");
    }
}
