using KubePizza.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace KubePizza.Core.Implementations
{
    /// <summary>
    /// Provides a wrapper around the system console for input and output operations.
    /// </summary>
    public class SystemConsole : IConsole
    {
        /// <summary>
        /// Writes a message to the console with the specified text color.
        /// </summary>
        /// <param name="message">The message to write to the console.</param>
        /// <param name="textColor">The color to use for the text.</param>
        public void Write(string? message = null, ConsoleColor textColor = ConsoleColor.White)
        {
            System.Console.ForegroundColor = textColor;
            System.Console.Write(message);
            System.Console.ResetColor();
        }

        /// <summary>
        /// Writes a message to the console with the specified text color, followed by a line terminator.
        /// </summary>
        /// <param name="message">The message to write to the console.</param>
        /// <param name="textColor">The color to use for the text.</param>
        public void WriteLine(string? message = null, ConsoleColor textColor = ConsoleColor.White)
        {
            System.Console.ForegroundColor = textColor;
            System.Console.WriteLine(message);
            System.Console.ResetColor();
        }

        /// <summary>
        /// Reads a line of characters from the console input stream.
        /// </summary>
        /// <returns>The next line of characters from the input stream, or <c>null</c> if no more lines are available.</returns>
        public string? ReadLine()
        {
            return System.Console.ReadLine();
        }
    }
}
