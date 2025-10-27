using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;

namespace _04_TabCompletion.Commands;

internal abstract class CommandBase : Command
{
    protected readonly Option<string> outputOption;
    protected readonly IServiceProvider serviceProvider;

    public CommandBase(string name, string description, IServiceProvider serviceProvider) : base(name, description)
    {
        this.serviceProvider = serviceProvider;

        outputOption = new Option<string>("--output", ["-o"]);
        outputOption.DefaultValueFactory = _ => "table";
        outputOption.AcceptOnlyFromAmong("table", "json", "yaml");

        this.Options.Add(outputOption);

    }
}
