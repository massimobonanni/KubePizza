using KubePizza.Console.Commands.Order;
using KubePizza.Console.Commands.Topping;
using KubePizza.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;

namespace KubePizza.Console.Commands;

internal class RootCommand : System.CommandLine.RootCommand
{
    public RootCommand(IServiceProvider serviceProvider, IConsole console) :
        base("kubepizza — manage your pizza orders like a pro 🍕")
    {
        this.Subcommands.Add(new OrderCommand(serviceProvider, console));
        this.Subcommands.Add(new ToppingCommand(serviceProvider, console));
    }
}
