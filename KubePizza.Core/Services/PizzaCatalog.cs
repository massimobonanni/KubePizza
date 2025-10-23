using System;
using System.Collections.Generic;
using System.Text;

namespace KubePizza.Core.Services;

public class PizzaCatalog : IPizzaCatalog
{
    public IReadOnlyList<string> Pizzas { get; } = new[]
    {
        "margherita", "diavola", "capricciosa", "quattroformaggi", "vegetariana"
    };

    public IReadOnlyList<string> AllToppings { get; } = new[]
    {
        "basil", "mozzarella", "bufala", "olive", "mushrooms", "onions",
        "peppers", "anchovies", "artichokes", "ham", "salami", "chili"
    };

    // Inventario iniziale (mutabile)
    public HashSet<string> Inventory { get; } = new(StringComparer.OrdinalIgnoreCase)
    {
        "basil", "mozzarella", "olive", "mushrooms", "onions", "salami"
    };

    public IReadOnlyList<Order> Orders { get; } = new[]
    {
        new Order("101", "margherita", "large",   "delivered"),
        new Order("102", "diavola",    "medium",  "preparing"),
        new Order("103", "capricciosa","small",   "open"),
        new Order("104", "vegetariana","large",   "cancelled"),
    };

    public IEnumerable<string> GetRecommendedToppingsFor(string pizza)
    {
        IEnumerable<string> recommended = pizza.ToLowerInvariant() switch
        {
            "margherita" => new[] { "basil", "mozzarella", "bufala" },
            "diavola" => new[] { "mozzarella", "chili", "onions" },
            "capricciosa" => new[] { "artichokes", "ham", "mushrooms", "olive" },
            "quattroformaggi" => new[] { "mozzarella", "bufala" },
            "vegetariana" => new[] { "mushrooms", "peppers", "onions", "olive" },
            _ => AllToppings
        };
        return recommended.Where(t => Inventory.Contains(t)); // solo se disponibili
    }
}

public record Order(string Id, string Pizza, string Size, string Status);
