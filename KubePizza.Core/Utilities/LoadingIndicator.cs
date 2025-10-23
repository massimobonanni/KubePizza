using System.Diagnostics;

namespace KubePizza.Core.Utilities;

/// <summary>
/// Provides a console loading indicator that displays rotating characters while an operation is in progress.
/// </summary>
public class LoadingIndicator : IDisposable
{
    /// <summary>
    /// The character array used for the spinner style animation.
    /// </summary>
    private static readonly char[] SpinnerChars = { '|', '/', '-', '\\' };
    
    /// <summary>
    /// The character array used for the dots style animation.
    /// </summary>
    private static readonly char[] DotsChars = { '.', 'o', 'O', 'o' };
    
    /// <summary>
    /// The character array used for the arrow style animation.
    /// </summary>
    private static readonly char[] ArrowChars = { '←', '↖', '↑', '↗', '→', '↘', '↓', '↙' };
    
    /// <summary>
    /// The character array used for the braille style animation.
    /// </summary>
    private static readonly char[] BrailleChars = { '⠋', '⠙', '⠹', '⠸', '⠼', '⠴', '⠦', '⠧', '⠇', '⠏' };

    /// <summary>
    /// The character array being used for the current animation style.
    /// </summary>
    private readonly char[] _characters;
    
    /// <summary>
    /// The message to display alongside the loading indicator.
    /// </summary>
    private readonly string _message;
    
    /// <summary>
    /// The interval in milliseconds between character changes in the animation.
    /// </summary>
    private readonly int _interval;
    
    /// <summary>
    /// The cancellation token source used to stop the animation task.
    /// </summary>
    private readonly CancellationTokenSource _cancellationTokenSource;
    
    /// <summary>
    /// The background task that handles the loading indicator animation.
    /// </summary>
    private readonly Task _animationTask;
    
    /// <summary>
    /// The current index in the character array for the animation frame.
    /// </summary>
    private int _currentIndex;
    
    /// <summary>
    /// Indicates whether this instance has been disposed.
    /// </summary>
    private bool _disposed;
    
    /// <summary>
    /// The total elapsed time in milliseconds when the loading indicator was stopped.
    /// </summary>
    private double _elapsedMillisecondsSecond;

    /// <summary>
    /// Gets the available loading indicator styles.
    /// </summary>
    public enum Style
    {
        Spinner,
        Dots,
        Arrow,
        Braille
    }

    /// <summary>
    /// Initializes a new loading indicator with the specified style and message.
    /// </summary>
    /// <param name="message">The message to display alongside the indicator.</param>
    /// <param name="style">The style of the loading indicator.</param>
    /// <param name="intervalMs">The interval between character changes in milliseconds.</param>
    public LoadingIndicator(string message = "Loading", Style style = Style.Spinner, int intervalMs = 100)
    {
        _message = message;
        _interval = intervalMs;
        _characters = style switch
        {
            Style.Dots => DotsChars,
            Style.Arrow => ArrowChars,
            Style.Braille => BrailleChars,
            _ => SpinnerChars
        };

        _cancellationTokenSource = new CancellationTokenSource();
        _animationTask = Task.Run(AnimateAsync, _cancellationTokenSource.Token);
    }

    /// <summary>
    /// Starts the loading indicator animation.
    /// </summary>
    /// <returns>A task representing the animation operation.</returns>
    private async Task AnimateAsync()
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            // Hide cursor
            System.Console.CursorVisible = false;
            var originalLeft = System.Console.CursorLeft;
            var originalTop = System.Console.CursorTop;

            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                // Reset cursor position
                System.Console.SetCursorPosition(originalLeft, originalTop);
                
                // Display current frame
                var elapsed = stopwatch.Elapsed;
                var character = _characters[_currentIndex % _characters.Length];
                var timeDisplay = $" ({elapsed.TotalSeconds:F1}s)";
                
                System.Console.Write($"{character} {_message}{timeDisplay}");
                
                // Move to next character
                _currentIndex++;
                
                await Task.Delay(_interval, _cancellationTokenSource.Token);
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when stopping the indicator
        }
        finally
        {
            _elapsedMillisecondsSecond = stopwatch.Elapsed.TotalMilliseconds;
            // Show cursor
            System.Console.CursorVisible = true;
        }
    }

    /// <summary>
    /// Stops the loading indicator and clears the display.
    /// </summary>
    public void Stop()
    {
        if (_disposed) return;

        _cancellationTokenSource.Cancel();
        
        try
        {
            _animationTask.Wait(1000); // Wait up to 1 second for animation to stop
        }
        catch (AggregateException ex) when (ex.InnerExceptions.All(e => e is OperationCanceledException))
        {
            // Expected cancellation exceptions
        }

        // Clear the loading indicator line
        var currentLeft = System.Console.CursorLeft;
        var currentTop = System.Console.CursorTop;
        
        System.Console.SetCursorPosition(0, currentTop);
        System.Console.Write(new string(' ', System.Console.WindowWidth - 1));
        System.Console.SetCursorPosition(0, currentTop);
        
        System.Console.CursorVisible = true;
    }

    /// <summary>
    /// Stops the loading indicator and displays a completion message.
    /// </summary>
    /// <param name="completionMessage">The message to display upon completion.</param>
    /// <param name="showTimeTaken">Whether to include the elapsed time in the completion message.</param>
    /// <param name="color">The color for the completion message.</param>
    public void Complete(string completionMessage, bool showTimeTaken, ConsoleColor color = ConsoleColor.Green)
    {
        Stop();
        if (showTimeTaken)
        {
            completionMessage += $" (Completed in {_elapsedMillisecondsSecond / 1000:F1}s)";
        }
        ConsoleUtility.WriteLine(completionMessage, color);
    }

    /// <summary>
    /// Disposes the loading indicator and stops the animation.
    /// </summary>
    public void Dispose()
    {
        if (_disposed) return;

        Stop();
        _cancellationTokenSource.Dispose();
        _disposed = true;
    }
}