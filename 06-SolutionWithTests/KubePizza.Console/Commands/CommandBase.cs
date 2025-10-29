using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;

namespace KubePizza.Console.Commands;

internal abstract class CommandBase : Command
{
    protected readonly Option<string> outputOption;
    protected readonly IServiceProvider serviceProvider;

    public CommandBase(string name, string description, IServiceProvider serviceProvider) : base(name, description)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        this.serviceProvider = serviceProvider;

        outputOption = new Option<string>("--output", ["-o"])
        {
            Description = "Output format for the command result (table, json, or yaml)."
        };
        outputOption.DefaultValueFactory = _ => "table";
        outputOption.AcceptOnlyFromAmong("table", "json", "yaml");

        this.Options.Add(outputOption);

    }
}
