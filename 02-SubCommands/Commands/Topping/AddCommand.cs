using KubePizza.Core.Utilities;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;
using System.Xml.Linq;

namespace _02_SubCommands.Commands.Topping
{
    internal class AddCommand : CommandBase
    {
        private readonly Option<string> nameOption;

        public AddCommand() : base("add", "Add a new topping")
        {
            nameOption = new Option<string>("--name")
            {
                Description = "Name of the topping",
                Required = true
            };

            this.Options.Add(nameOption);

            this.SetAction(CommandHandler);
        }

        private async Task CommandHandler(ParseResult parseResult, CancellationToken cancellationToken)
        {
            var name = parseResult.GetRequiredValue(nameOption);

            ConsoleUtility.WriteLine($"🥫 Added topping: {name}", ConsoleColor.Green);
        }
    }
}
