using _04_TabCompletion.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace _04_TabCompletion.Commands.Topping;

internal class ToppingCommand : CommandBase
{
    public ToppingCommand(IServiceProvider serviceProvider) : base("topping", "Manage available toppings", serviceProvider)
    {
        this.Aliases.Add("t");

        this.Subcommands.Add(new AddCommand(serviceProvider));
        this.Subcommands.Add(new ListCommand(serviceProvider));
    }
}
