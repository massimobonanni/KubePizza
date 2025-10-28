using System.CommandLine;

namespace _05_CustomHelp.Help;

/// <summary>
/// Provides examples for System.CommandLine commands based on their hierarchical path.
/// </summary>
/// <remarks>
/// This class stores examples in a dictionary keyed by command paths and provides
/// functionality to retrieve examples for specific commands and generate command paths.
/// </remarks>
internal sealed class ExamplesProvider
{
    /// <summary>
    /// Dictionary mapping command paths to their associated examples.
    /// </summary>
    private readonly Dictionary<string, string[]> _examplesByPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExamplesProvider"/> class.
    /// </summary>
    /// <param name="examplesByPath">
    /// A dictionary where keys are command paths (space-separated command hierarchy)
    /// and values are arrays of example strings for those commands.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="examplesByPath"/> is null.
    /// </exception>
    public ExamplesProvider(Dictionary<string, string[]> examplesByPath)
        => _examplesByPath = examplesByPath;

    /// <summary>
    /// Retrieves examples for the specified command.
    /// </summary>
    /// <param name="command">The command to get examples for.</param>
    /// <returns>
    /// An array of example strings for the command, or an empty array if no examples are found.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="command"/> is null.
    /// </exception>
    public string[] GetExamplesFor(Command command)
    {
        var path = GetPath(command);
        return _examplesByPath.TryGetValue(path, out var ex) ? ex : Array.Empty<string>();
    }

    /// <summary>
    /// Generates a hierarchical path string for the specified command.
    /// </summary>
    /// <param name="command">The command to generate a path for.</param>
    /// <returns>
    /// A space-separated string representing the command hierarchy from root to the specified command.
    /// For example, "myapp subcommand action" for a nested command structure.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="command"/> is null.
    /// </exception>
    /// <remarks>
    /// The method traverses up the command hierarchy to build the complete path,
    /// then reverses the order to present the path from root to leaf.
    /// </remarks>
    public static string GetPath(Command command)
    {
        var parts = new List<string>();
        var current = command;
        while (current is not null)
        {
            parts.Add(current.Name);
            current = current.Parents.FirstOrDefault() as Command;
        }
        parts.Reverse();
        return string.Join(' ', parts);
    }
}
