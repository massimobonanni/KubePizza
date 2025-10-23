using System;
using System.Collections.Generic;
using System.Text;

namespace _02_SubCommands.Commands.Topping
{
    internal class ToppingCommand : CommandBase
    {
        public ToppingCommand() : base("topping", "Manage available toppings")
        {
            this.Aliases.Add("t");

            this.Subcommands.Add(new AddCommand());
            this.Subcommands.Add(new ListCommand());
        }
    }
}
