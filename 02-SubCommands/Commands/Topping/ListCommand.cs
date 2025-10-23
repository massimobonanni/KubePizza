using KubePizza.Core.Utilities;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;

namespace _02_SubCommands.Commands.Topping
{
    internal class ListCommand : CommandBase
    {

        public ListCommand() : base("list", "List all available toppings")
        {
            this.SetAction(CommandHandler);
        }

        private async Task CommandHandler(ParseResult parseResult, CancellationToken cancellationToken)
        {
            ConsoleUtility.WriteLine("Available toppings:", ConsoleColor.Green);
            ConsoleUtility.WriteLine("\t- basil");
            ConsoleUtility.WriteLine("\t- mozzarella");
            ConsoleUtility.WriteLine("\t- olives");
            ConsoleUtility.WriteLine("\t- mushrooms");
        }
    }
}
