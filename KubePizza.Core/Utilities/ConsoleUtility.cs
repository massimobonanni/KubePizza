using Figgle;

namespace KubePizza.Core.Utilities;

/// <summary>
/// Provides utility methods for enhanced console output operations with color support and formatting.
/// </summary>
internal static class ConsoleUtility
{
    /// <summary>
    /// Writes a message to the console followed by a line terminator, with optional foreground color.
    /// </summary>
    /// <param name="message">The message to write to the console. Defaults to an empty string.</param>
    /// <param name="foregroundColor">The foreground color to use for the text. Defaults to <see cref="ConsoleColor.White"/>.</param>
    public static void WriteLine(string message = "", ConsoleColor foregroundColor = ConsoleColor.White)
    {
        var currentForegroundColor = System.Console.ForegroundColor;
        System.Console.ForegroundColor = foregroundColor;
        System.Console.WriteLine(message);
        System.Console.ForegroundColor = currentForegroundColor;
    }

    /// <summary>
    /// Writes a message to the console with optional foreground color, without a line terminator.
    /// </summary>
    /// <param name="message">The message to write to the console. Defaults to an empty string.</param>
    /// <param name="foregroundColor">The foreground color to use for the text. Defaults to <see cref="ConsoleColor.White"/>.</param>
    public static void Write(string message = "", ConsoleColor foregroundColor = ConsoleColor.White)
    {
        var currentForegroundColor = System.Console.ForegroundColor;
        System.Console.ForegroundColor = foregroundColor;
        System.Console.Write(message);
        System.Console.ForegroundColor = currentForegroundColor;
    }

    /// <summary>
    /// Writes a timestamped message to the console followed by a line terminator, with optional foreground color.
    /// The timestamp format is [HH:mm:ss.fff].
    /// </summary>
    /// <param name="message">The message to write to the console. Defaults to an empty string.</param>
    /// <param name="foregroundColor">The foreground color to use for the text. Defaults to <see cref="ConsoleColor.White"/>.</param>
    public static void WriteLineWithTimestamp(string message = "", ConsoleColor foregroundColor = ConsoleColor.White)
    {
        WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] - {message}", foregroundColor);
    }

    /// <summary>
    /// Writes a timestamped message to the console with optional foreground color, without a line terminator.
    /// The timestamp format is [HH:mm:ss.fff].
    /// </summary>
    /// <param name="message">The message to write to the console. Defaults to an empty string.</param>
    /// <param name="foregroundColor">The foreground color to use for the text. Defaults to <see cref="ConsoleColor.White"/>.</param>
    public static void WriteWithTimestamp(string message = "", ConsoleColor foregroundColor = ConsoleColor.White)
    {
        Write($"[{DateTime.Now:HH:mm:ss.fff}] - {message}", foregroundColor);
    }

    /// <summary>
    /// Writes the application banner "Promptus Maximus" to the console in ASCII art format using Figgle fonts.
    /// The banner is displayed in green color with blank lines before and after for visual separation.
    /// </summary>
    public static void WriteApplicationBanner()
    {
        WriteLine();
        WriteLine(Figgle.Fonts.FiggleFonts.Standard.Render("Promptus Maximus"), ConsoleColor.Green);
        WriteLine();
    }
}

