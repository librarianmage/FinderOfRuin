using HarmonyLib;
using ConsoleLib.Console;

namespace FinderOfRuin.HarmonyPatches
{
    [HarmonyPatch(typeof(XRL.LoreGenerator),nameof(XRL.LoreGenerator.RuinOfHouseIsnerLore))]
    class LorePatcher
    {
        static string Postfix(string lore)
        {
            return lore
                .Replace("masterwork", "&cmasterwork&y")
                .Replace("Ruin", "&rRuin&y")
                .Replace("House Isner", "&MHouse Isner&y");
        }
    }
}
