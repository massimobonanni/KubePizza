using _04_TabCompletion.Commands.Order;
using _04_TabCompletion.Commands.Topping;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;

namespace _04_TabCompletion.Commands;

internal class RootCommand : System.CommandLine.RootCommand
{
    public RootCommand(IServiceProvider serviceProvider) : base("kubepizza — manage your pizza orders like a pro 🍕")
    {
        this.Subcommands.Add(new OrderCommand(serviceProvider));
        this.Subcommands.Add(new ToppingCommand(serviceProvider));
    }
}
