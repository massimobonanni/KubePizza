using _02_SubCommands.Commands.Order;
using _02_SubCommands.Commands.Topping;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;

namespace _02_SubCommands.Commands
{
    internal class RootCommand : System.CommandLine.RootCommand
    {
        public RootCommand() : base("kubepizza — manage your pizza orders like a pro 🍕")
        {
            this.Subcommands.Add(new OrderCommand());
            this.Subcommands.Add(new ToppingCommand());
        }
    }
}
