using _03_Validators.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;

namespace _03_Validators.Commands.Order;

internal class OrderCommand : CommandBase
{
    public OrderCommand(IServiceProvider serviceProvider) : base("order", "Manage pizza orders", serviceProvider)
    {
        this.Aliases.Add("o");

        this.Subcommands.Add(new CreateCommand(serviceProvider));
        this.Subcommands.Add(new ListCommand(serviceProvider));

    }
}
