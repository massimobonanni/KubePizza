using _05_CustomHelp.Help;
using System.CommandLine;
using System.CommandLine.Help;

namespace _05_CustomHelp.Commands.Order;

internal class OrderCommand : CommandBase
{
    public OrderCommand(IServiceProvider serviceProvider) : base("order", "Manage pizza orders", serviceProvider)
    {
        this.Aliases.Add("o");

        this.Subcommands.Add(new CreateCommand(serviceProvider));
        this.Subcommands.Add(new ListCommand(serviceProvider));

        for (int i = 0; i < this.Options.Count; i++)
        {
            if (this.Options[i] is HelpOption defaultHelpOption)
            {
                defaultHelpOption.Action = new CustomHelpAction((HelpAction)defaultHelpOption.Action!);
                break;
            }
        }
    }
}
