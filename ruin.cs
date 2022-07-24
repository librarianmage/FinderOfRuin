using HarmonyLib;
using XRL;
using XRL.UI;

namespace FinderOfRuin.HarmonyPatches
{
    [HarmonyPatch(typeof(LoreGenerator))]
    [HarmonyPatch(nameof(LoreGenerator.RuinOfHouseIsnerLore))]
    class LorePatcher
    {
        static void Postfix(ref string lore)
        {

            if (Options.GetOption("Books_FinderOfRuin_Color", "No") == "Yes")
            {
                lore = lore
                    .Replace("masterwork", "&cmasterwork&y")
                    .Replace("Ruin", "&rRuin&y")
                    .Replace("House Isner", "&MHouse Isner&y");
            }
            if (Options.GetOption("Books_FinderOfRuin_Capitalization", "No") == "Yes")
            {
                lore = lore
                    .Replace("masterwork", "MASTERWORK")
                    .Replace("Ruin", "RUIN")
                    .Replace("House Isner", "HOUSE ISNER");
            }

        }
    }
}
