using KubePizza.Core.Utilities;

namespace System.Threading;

/// <summary>
/// Extension methods for easy integration of loading indicators with async operations.
/// </summary>
public static class LoadingIndicatorExtensions
{
    /// <summary>
    /// Executes an async operation with a loading indicator.
    /// </summary>
    /// <typeparam name="T">The return type of the async operation.</typeparam>
    /// <param name="task">The task to execute.</param>
    /// <param name="message">The loading message to display.</param>
    /// <param name="style">The style of the loading indicator.</param>
    /// <param name="completionMessage">The message to display when completed successfully.</param>
    /// <param name="completionColor">The color to use for the completion message.</param>
    /// <param name="showTimeTaken">Whether to display the time taken for the operation.</param>
    /// <param name="intervalMs">The interval between character changes in milliseconds.</param>
    /// <returns>The result of the async operation.</returns>
    /// <exception cref="Exception">Rethrows any exception that occurs during task execution.</exception>
    public static async Task<T> WithLoadingIndicator<T>(
        this Task<T> task,
        string message = "Processing",
        LoadingIndicator.Style style = LoadingIndicator.Style.Spinner,
        string? completionMessage = null,
        ConsoleColor completionColor = ConsoleColor.Green,
        bool showTimeTaken = false,
        int intervalMs = 100)
    {
        using var indicator = new LoadingIndicator(message, style, intervalMs);

        try
        {
            var result = await task;

            if (completionMessage != null)
            {
                indicator.Complete(completionMessage, showTimeTaken, completionColor);
            }
            else
            {
                indicator.Stop();
            }

            return result;
        }
        catch
        {
            indicator.Complete("Failed", showTimeTaken,ConsoleColor.Red);
            throw;
        }
    }

    /// <summary>
    /// Executes an async operation with a loading indicator (no return value).
    /// </summary>
    /// <param name="task">The task to execute.</param>
    /// <param name="message">The loading message to display.</param>
    /// <param name="style">The style of the loading indicator.</param>
    /// <param name="completionMessage">The message to display when completed successfully.</param>
    /// <param name="completionColor">The color to use for the completion message.</param>
    /// <param name="showTimeTaken">Whether to display the time taken for the operation.</param>
    /// <param name="intervalMs">The interval between character changes in milliseconds.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="Exception">Rethrows any exception that occurs during task execution.</exception>
    public static async Task WithLoadingIndicator(
        this Task task,
        string message = "Processing",
        LoadingIndicator.Style style = LoadingIndicator.Style.Spinner,
        string? completionMessage = null,
        ConsoleColor completionColor = ConsoleColor.Green,
        bool showTimeTaken = false,
        int intervalMs = 100)
    {
        using var indicator = new LoadingIndicator(message, style, intervalMs);

        try
        {
            await task;

            if (completionMessage != null)
            {
                indicator.Complete(completionMessage, showTimeTaken, completionColor);
            }
            else
            {
                indicator.Stop();
            }
        }
        catch
        {
            indicator.Complete("Failed", showTimeTaken, ConsoleColor.Red);
            throw;
        }
    }
}