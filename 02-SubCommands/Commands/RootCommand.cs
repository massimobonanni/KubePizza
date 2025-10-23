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
            var outputOption = new Option<string>("--output", ["-o"]);
            outputOption.DefaultValueFactory = (a) => "table";
            outputOption.AcceptOnlyFromAmong("table", "json", "yaml");

            this.Options.Add(outputOption);

            this.Subcommands.Add(new OrderCommand());
            this.Subcommands.Add(new CreateCommand());
        }
    }
}
