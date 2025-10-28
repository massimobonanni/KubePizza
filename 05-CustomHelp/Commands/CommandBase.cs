using System.CommandLine;

namespace _05_CustomHelp.Commands;

internal abstract class CommandBase : Command
{
    protected readonly Option<string> outputOption;
    protected readonly Option<bool> noColorOption;
    protected readonly IServiceProvider serviceProvider;

    public CommandBase(string name, string description, IServiceProvider serviceProvider) : base(name, description)
    {
        this.serviceProvider = serviceProvider;

        outputOption = new Option<string>("--output", ["-o"]);
        outputOption.DefaultValueFactory = _ => "table";
        outputOption.AcceptOnlyFromAmong("table", "json", "yaml");

        noColorOption = new Option<bool>("--no-color");
        noColorOption.Description = "Disable ANSI colors/emojis in help and output.";
        noColorOption.DefaultValueFactory = _ => false;

        this.Options.Add(outputOption);
        this.Options.Add(noColorOption);
    }
}
