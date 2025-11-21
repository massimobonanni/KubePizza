using KubePizza.Core.Utilities;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Help;
using System.CommandLine.Invocation;
using System.Text;

namespace _05_CustomHelp.Help;

/// <summary>
/// Custom help action that extends the default help functionality by displaying
/// command-specific examples after the standard help text.
/// </summary>
internal class CustomHelpAction : SynchronousCommandLineAction
{
    private readonly HelpAction _defaultHelp;
    private readonly ExamplesProvider _examplesProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomHelpAction"/> class.
    /// </summary>
    /// <param name="action">The default help action to delegate standard help rendering to.</param>
    public CustomHelpAction(HelpAction action)
    {
        _defaultHelp = action;
        _examplesProvider = new ExamplesProvider(Constants.Examples);
    }

    /// <summary>
    /// Invokes the help action, displaying both the default help text and custom examples.
    /// </summary>
    /// <param name="parseResult">The parse result containing the command context.</param>
    /// <returns>The exit code from the default help action.</returns>
    public override int Invoke(ParseResult parseResult)
    { 
        // Display the default help text first
        int result = _defaultHelp.Invoke(parseResult);

        // Retrieve examples specific to the current command
        var examples = _examplesProvider.GetExamplesFor(parseResult.CommandResult.Command);

        // If examples are available, display them with custom formatting
        if (examples.Length > 0)
        {
            Console.WriteLine();
            ConsoleUtility.WriteLine("You can try these commands:", ConsoleColor.DarkGreen);
            foreach (var e in examples)
                ConsoleUtility.WriteLine($"\t{e}", ConsoleColor.Blue);
        }

        return result;
    }
}
