using HarmonyLib;
using ConsoleLib.Console;

namespace FinderOfRuin.HarmonyPatches
{
    [HarmonyPatch(typeof(XRL.LoreGenerator),nameof(XRL.LoreGenerator.RuinOfHouseIsnerLore))]
    class LorePatcher
    {
        static string Postfix(string __result)
        {
            string lore;
            lore = __result
                .Replace("masterwork", "&cmasterwork&y")
                .Replace("Ruin", "&rRuin&y")
                .Replace("House Isner", "&MHouse Isner&y");
            UnityEngine.Debug.Log($"Found the pistol! {lore}");
            return lore;
        }
    }
}
