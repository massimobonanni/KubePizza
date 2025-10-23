using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;

namespace _02_SubCommands.Commands
{
    internal abstract class CommandBase : Command
    {
        protected readonly Option<string> outputOption;

        public CommandBase(string name, string description) : base(name, description)
        {
            outputOption = new Option<string>("--output", ["-o"]);
            outputOption.DefaultValueFactory = _ => "table";
            outputOption.AcceptOnlyFromAmong("table", "json", "yaml");

            this.Options.Add(outputOption);

        }
    }
}
