using System;
using System.Collections.Generic;
using System.Text;

namespace _05_CustomHelp
{
    internal static class Constants
    {
        internal static Dictionary<string, string[]> Examples = new Dictionary<string, string[]>
        {
            // root
            ["kubepizza"] = new[]
            {
                "kubepizza --help",
                "kubepizza order --help",
                "kubepizza order create --pizza margherita --size large --toppings basil,mozzarella" ,               "kubepizza order list --status open --output json"
            },
            // order
            ["kubepizza order"] = new[]
            {
                "kubepizza order create --pizza diavola --size medium",
                "kubepizza order list --status delivered"
            },
            // order create
            ["kubepizza order create"] = new[]
            {
                "kubepizza order create --pizza margherita --size large --toppings basil,mozzarella",
                "kubepizza order create --pizza vegetariana --toppings mushrooms,peppers "
            },
            // order list
            ["kubepizza order list"] = new[]
            {
                "kubepizza order list --status preparing --output yaml",
                "kubepizza order list --status all --output table"
            }
        };
    }
}
