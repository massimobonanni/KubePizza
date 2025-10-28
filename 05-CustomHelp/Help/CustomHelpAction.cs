using KubePizza.Core.Utilities;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Help;
using System.CommandLine.Invocation;
using System.Text;

namespace _05_CustomHelp.Help;

internal class CustomHelpAction : SynchronousCommandLineAction
{
    private readonly HelpAction _defaultHelp;
    private readonly ExamplesProvider _examplesProvider;

    public CustomHelpAction(HelpAction action)
    {
        _defaultHelp = action;
        _examplesProvider = new ExamplesProvider(Constants.Examples);
    }

    public override int Invoke(ParseResult parseResult)
    { 
        int result = _defaultHelp.Invoke(parseResult);

        var examples = _examplesProvider.GetExamplesFor(parseResult.CommandResult.Command);

        if (examples.Length > 0)
        {
            Console.WriteLine();
            ConsoleUtility.WriteLine("Examples",ConsoleColor.DarkGreen);
            foreach (var e in examples)
                ConsoleUtility.WriteLine($"\t{e}",ConsoleColor.Blue);
        }

        return result;

    }
}
