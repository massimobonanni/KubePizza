using KubePizza.Console.Commands;
using KubePizza.Core.Interfaces;
using KubePizza.Core.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;
using System.Xml.Linq;

namespace KubePizza.Console.Commands.Topping;

internal class AddCommand : CommandBase
{
    private readonly Option<string> nameOption;

    public AddCommand(IServiceProvider serviceProvider, IConsole console) :
        base("add", "Add a new topping", serviceProvider, console)
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

        console.WriteLine($"Added topping: {name}", ConsoleColor.Green);
    }
}
