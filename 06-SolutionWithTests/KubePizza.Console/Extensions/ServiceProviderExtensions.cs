using KubePizza.Console.Commands;
using KubePizza.Console.Commands.Order;
using KubePizza.Core.Interfaces;
using KubePizza.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;

namespace System
{
    internal static class ServiceProviderExtensions
    {
        public static IConsole GetConsole(this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetRequiredService<IConsole>();
        }

        public static IPizzaCatalog GetPizzaCatalog(this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetRequiredService<IPizzaCatalog>();
        }

    }
}
