
namespace KubePizza.Core.Services
{
    public interface IPizzaCatalog
    {
        IReadOnlyList<string> AllToppings { get; }
        HashSet<string> Inventory { get; }
        IReadOnlyList<Order> Orders { get; }
        IReadOnlyList<string> Pizzas { get; }

        IEnumerable<string> GetRecommendedToppingsFor(string pizza);
    }
}