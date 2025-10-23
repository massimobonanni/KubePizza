using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;

namespace _02_SubCommands.Commands
{
    internal class CreateCommand : Command
    {
        public CreateCommand() : base("create", "Create a new pizza order")
        {
            var pizzaOption = new Option<string>("--pizza")
            {
                Description = "Type of pizza(e.g.margherita, diavola)"
            };
            pizzaOption.Required = true;
            
            var sizeOption = new Option<string>("--size")
            {
                DefaultValueFactory = (a) => "medium",
            };
            sizeOption.AcceptOnlyFromAmong("small", "medium", "large");
           
            var toppingsOption = new Option<string[]>("--toppings")
            {
                Description = "List of extra toppings (comma-separated).",
                Arity = ArgumentArity.ZeroOrMore,
                AllowMultipleArgumentsPerToken = true
            };
            
            var deliveryOption = new Option<bool>("--delivery")
            {
                Description = "Specify if the order is for delivery (default: true).",
                DefaultValueFactory = (a) => true,
            };

            this.Options.Add(pizzaOption);
            this.Options.Add(sizeOption);
            this.Options.Add(toppingsOption);
            this.Options.Add(deliveryOption);

            this.SetAction(CommandHandler);
        }

        private async Task CommandHandler(ParseResult parseResult, CancellationToken cancellationToken)
        {
            
        }
    }
}
