using _05_CustomHelp.Commands.Order;
using _05_CustomHelp.Commands.Topping;
using _05_CustomHelp.Help;
using System.CommandLine;
using System.CommandLine.Help;

namespace _05_CustomHelp.Commands;

internal class RootCommand : System.CommandLine.RootCommand
{
    public RootCommand(IServiceProvider serviceProvider) : base()
    {
        this.Description = "Manage pizza orders using a familiar, kubectl-like CLI. " +
                              "This demo focuses on custom help output and guidance.";

        this.Subcommands.Add(new OrderCommand(serviceProvider));
        this.Subcommands.Add(new ToppingCommand(serviceProvider));

        for (int i = 0; i < this.Options.Count; i++)
        {   
            if (this.Options[i] is HelpOption defaultHelpOption)
            {
                defaultHelpOption.Action = new CustomHelpAction((HelpAction)defaultHelpOption.Action!);
            }
            if (this.Options[i] is VersionOption defaultVersionOption)
            {
                defaultVersionOption.Action = new CustomVersionAction();
            }
        }
    }
}
