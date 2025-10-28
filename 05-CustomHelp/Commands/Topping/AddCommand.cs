using KubePizza.Core.Utilities;
using System.CommandLine;

namespace _05_CustomHelp.Commands.Topping;

internal class AddCommand : CommandBase
{
    private readonly Option<string> nameOption;

    public AddCommand(IServiceProvider serviceProvider) : base("add", "Add a new topping", serviceProvider)
    {
        nameOption = new Option<string>("--name")
        {
            Description = "Name of the topping",
            Required = true
        };

        this.Options.Add(nameOption);

        this.SetAction(CommandHandler);
    }

    private async Task CommandHandler(ParseResult parseResult, CancellationToken cancellationToken)
    {
        var name = parseResult.GetRequiredValue(nameOption);

        ConsoleUtility.WriteLine($"🥫 Added topping: {name}", ConsoleColor.Green);
    }
}
