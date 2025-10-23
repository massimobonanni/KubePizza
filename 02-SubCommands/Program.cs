using _02_SubCommands.Commands;
using KubePizza.Core.Utilities;
using System.CommandLine;

ConsoleUtility.WriteApplicationBanner();

var rootCommand = new _02_SubCommands.Commands.RootCommand();

ParseResult parseResult = rootCommand.Parse(args);
return await parseResult.InvokeAsync();