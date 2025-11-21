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
            ["order"] = new[]
            {
                "kp05 order create --pizza diavola --size medium",
                "kp05 order list --status delivered"
            },
            // order create
            ["order create"] = new[]
            {
                "kp05 order create --pizza margherita --size large --toppings basil,mozzarella",
                "kp05 order create --pizza vegetariana --toppings mushrooms,peppers "
            },
            // order list
            ["order list"] = new[]
            {
                "kp05 order list --status preparing --output yaml",
                "kp05 order list --status all --output table"
            }
        };
    }
}
