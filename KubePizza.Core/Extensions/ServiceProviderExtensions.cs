
using KubePizza.Core.Services;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods for <see cref="ServiceProvider"/> to simplify service retrieval for PromptusMaximus application services.
/// </summary>
internal static class ServiceProviderExtensions
{
   
    public static IPizzaCatalog GetModelsClient(this IServiceProvider provider)
    {
        return provider.GetRequiredService<IPizzaCatalog>();
    }

}
