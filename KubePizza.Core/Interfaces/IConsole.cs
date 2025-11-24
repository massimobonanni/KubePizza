using System;
using System.Collections.Generic;
using System.Text;

namespace KubePizza.Core.Interfaces
{
    /// <summary>
    /// Defines a contract for console input and output operations.
    /// </summary>
    public interface IConsole
    {
        /// <summary>
        /// Writes a message to the console with the specified text color.
        /// </summary>
        /// <param name="message">The message to write to the console.</param>
        /// <param name="textColor">The color to use for the text.</param>
        void Write(string? message = null, ConsoleColor textColor = ConsoleColor.White);

        /// <summary>
        /// Writes a message to the console with the specified text color, followed by a line terminator.
        /// </summary>
        /// <param name="message">The message to write to the console.</param>
        /// <param name="textColor">The color to use for the text.</param>
        void WriteLine(string? message = null, ConsoleColor textColor = ConsoleColor.White);

        /// <summary>
        /// Reads a line of characters from the console input stream.
        /// </summary>
        /// <returns>The next line of characters from the input stream, or <c>null</c> if no more lines are available.</returns>
        string? ReadLine();
    }
}
