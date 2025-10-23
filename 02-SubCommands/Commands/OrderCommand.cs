using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;

namespace _02_SubCommands.Commands
{
    internal class OrderCommand:Command
    {
        public OrderCommand():base("order", "Manage pizza orders")
        {
            this.Aliases.Add("o");

            this.SetAction(CommandHandler);
        }
        private async Task CommandHandler(ParseResult parseResult, CancellationToken cancellationToken)
        {

        }
    }
}
