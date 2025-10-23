using KubePizza.Core.Utilities;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Drawing;
using System.Text;

namespace _02_SubCommands.Commands.Order
{
    internal class CreateCommand : CommandBase
    {
        private readonly Option<string> pizzaOption;
        private readonly Option<string> sizeOption;
        private readonly Option<string[]> toppingsOption;
        private readonly Option<bool> deliveryOption;

        public CreateCommand() : base("create", "Create a new pizza order")
        {
            pizzaOption = new Option<string>("--pizza")
            {
                Description = "Type of pizza(e.g.margherita, diavola)"
            };
            pizzaOption.Required = true;
            
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
}
