namespace _05_CustomHelp.Commands.Topping;

internal class ToppingCommand : CommandBase
{
    public ToppingCommand(IServiceProvider serviceProvider) : base("topping", "Manage available toppings", serviceProvider)
    {
        this.Aliases.Add("t");

        this.Subcommands.Add(new AddCommand(serviceProvider));
        this.Subcommands.Add(new ListCommand(serviceProvider));
    }
}
