using KubePizza.Console.Commands;
using KubePizza.Core.Interfaces;
using KubePizza.Core.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;

namespace KubePizza.Console.Commands.Topping;

internal class ListCommand : CommandBase
{

    public ListCommand(IServiceProvider serviceProvider, IConsole console) : 
        base("list", "List all available toppings", serviceProvider, console)
    {
        this.SetAction(CommandHandler);
    }

    private async Task CommandHandler(ParseResult parseResult, CancellationToken cancellationToken)
    {
        console.WriteLine("Available toppings:", ConsoleColor.Green);
        console.WriteLine("\t- basil");
        console.WriteLine("\t- mozzarella");
        console.WriteLine("\t- olives");
        console.WriteLine("\t- mushrooms");
    }
}
