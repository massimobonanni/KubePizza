using KubePizza.Core.Utilities;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text;

namespace _05_CustomHelp.Help
{
    /// <summary>
    /// Custom version action that overrides the default version display behavior
    /// with a custom message instead of showing actual version information.
    /// </summary>
    internal class CustomVersionAction : SynchronousCommandLineAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomVersionAction"/> class.
        /// </summary>
        public CustomVersionAction()
        {
        }

        /// <summary>
        /// Invokes the version action, displaying a custom message instead of version details.
        /// </summary>
        /// <param name="parseResult">The parse result containing the command context.</param>
        /// <returns>Always returns 0 to indicate successful execution.</returns>
        override public int Invoke(ParseResult parseResult)
        {
            // Display a humorous custom message instead of the actual version information
            ConsoleUtility.WriteLine("This version is personal information. These things don't concern you!", ConsoleColor.Red);
            
            return 0;
        }
    }
}
