using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;

namespace _02_SubCommands.Commands.Order
{
    internal class OrderCommand : CommandBase
    {
        public OrderCommand() : base("order", "Manage pizza orders")
        {
            this.Aliases.Add("o");

            this.Subcommands.Add(new CreateCommand());
            this.Subcommands.Add(new ListCommand());

        }
    }
}
