using _03_Validators.Commands;
using KubePizza.Core.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;

namespace _03_Validators.Commands.Order;

internal class ListCommand : CommandBase
{
    private readonly Option<string> statusOption;

    public ListCommand(IServiceProvider serviceProvider) : base("list", "List all pizza orders", serviceProvider)
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

        ConsoleUtility.WriteLine($"Listing {status} orders (output: {output})", ConsoleColor.Green);
        ConsoleUtility.WriteLine();
        ConsoleUtility.WriteLine("ID   Pizza         Size    Status");
        ConsoleUtility.WriteLine("1    Margherita    Large   Delivered");
        ConsoleUtility.WriteLine("2    Diavola       Medium  Preparing");
    }
}
