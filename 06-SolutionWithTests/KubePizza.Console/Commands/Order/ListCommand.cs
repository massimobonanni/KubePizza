using KubePizza.Console.Commands;
using KubePizza.Core.Interfaces;
using KubePizza.Core.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;

namespace KubePizza.Console.Commands.Order;

internal class ListCommand : CommandBase
{
    private readonly Option<string> statusOption;

    public ListCommand(IServiceProvider serviceProvider, IConsole console) : 
        base("list", "List all pizza orders", serviceProvider, console)
    {
        statusOption = new Option<string>("--status")
        {
            Description = "Filter by order status (all, open, delivered, cancelled)",
            DefaultValueFactory = _ => "all"
        };
        statusOption.AcceptOnlyFromAmong("all", "open", "delivered", "cancelled");

        this.Options.Add(statusOption);

        this.SetAction(CommandHandler);
    }

    private async Task CommandHandler(ParseResult parseResult, CancellationToken cancellationToken)
    {
        var status = parseResult.GetValue(statusOption);
        var output = parseResult.GetValue<string>(outputOption);

        console.WriteLine($"Listing {status} orders (output: {output})", ConsoleColor.Green);
        console.WriteLine();
        console.WriteLine("ID   Pizza         Size    Status");
        console.WriteLine("1    Margherita    Large   Delivered");
        console.WriteLine("2    Diavola       Medium  Preparing");
    }
}
