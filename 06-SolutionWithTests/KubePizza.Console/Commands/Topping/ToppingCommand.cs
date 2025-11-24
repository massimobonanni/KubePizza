using KubePizza.Console.Commands;
using KubePizza.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace KubePizza.Console.Commands.Topping;

internal class ToppingCommand : CommandBase
{
    public ToppingCommand(IServiceProvider serviceProvider, IConsole console) :
        base("topping", "Manage available toppings", serviceProvider, console)
    {
        this.Aliases.Add("t");

        this.Subcommands.Add(new AddCommand(serviceProvider, console));
        this.Subcommands.Add(new ListCommand(serviceProvider, console));
    }
}
