using KubePizza.Console.Commands;
using KubePizza.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;

namespace KubePizza.Console.Commands.Order;

internal class OrderCommand : CommandBase
{
    public OrderCommand(IServiceProvider serviceProvider, IConsole console) :
        base("order", "Manage pizza orders", serviceProvider, console)
    {
        this.Aliases.Add("o");

        this.Subcommands.Add(new CreateCommand(serviceProvider,console));
        this.Subcommands.Add(new ListCommand(serviceProvider,console));

    }
}
