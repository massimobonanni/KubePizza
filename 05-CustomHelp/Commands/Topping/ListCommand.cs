using KubePizza.Core.Utilities;
using System.CommandLine;

namespace _05_CustomHelp.Commands.Topping;

internal class ListCommand : CommandBase
{

    public ListCommand(IServiceProvider serviceProvider) : base("list", "List all available toppings", serviceProvider)
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
