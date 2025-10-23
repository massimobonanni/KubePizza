using _03_Validators.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace _03_Validators.Commands.Topping;

internal class ToppingCommand : CommandBase
{
    public ToppingCommand(IServiceProvider serviceProvider) : base("topping", "Manage available toppings", serviceProvider)
    {
        this.Aliases.Add("t");

        this.Subcommands.Add(new AddCommand(serviceProvider));
        this.Subcommands.Add(new ListCommand(serviceProvider));
    }
}
