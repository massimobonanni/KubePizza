using KubePizza.Core.Services;
using KubePizza.Core.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.CommandLine;

ConsoleUtility.WriteApplicationBanner("KubePizza - 03");

/// <summary>
/// Configure dependency injection container with required services.
/// Registers session management, GitHub Models service, and client services as singletons.
/// </summary>
var serviceCollection = new ServiceCollection();
serviceCollection.TryAddSingleton<IPizzaCatalog, PizzaCatalog>();
var serviceProvider=serviceCollection.BuildServiceProvider();

var rootCommand = new _03_Validators.Commands.RootCommand(serviceProvider);

ParseResult parseResult = rootCommand.Parse(args);
return await parseResult.InvokeAsync();