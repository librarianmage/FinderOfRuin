using HarmonyLib;
using System;
using System.Text.RegularExpressions;
using XRL;
using XRL.UI;

namespace FinderOfRuin.HarmonyPatches
{
    [HarmonyPatch(typeof(LoreGenerator))]
    [HarmonyPatch(nameof(LoreGenerator.RuinOfHouseIsnerLore))]
    class LorePatcher
    {
        static void Postfix(ref string __result)
        {
            String Color = Options.GetOption("Books_FinderOfRuin_Color", "No");
            String Capitalization = Options.GetOption("Books_FinderOfRuin_Capitalization", "Default");

            if (Capitalization == "Key Words")
            {
                __result = __result
                    .Replace("masterwork", "MASTERWORK")
                    .Replace("Ruin", "RUIN")
                    .Replace("House Isner", "HOUSE ISNER");
            }
            else if (Capitalization == "Entire Clue") {
                Regex rx = new Regex(@"\{(.*)\}");
                MatchEvaluator evaluator = new MatchEvaluator(Capitalize);
                __result = rx.Replace(__result, evaluator);
            }

            if (Color == "Yes")
            {
                if (Capitalization == "Default")
                {
                    __result = __result
                        .Replace("masterwork", "&cmasterwork&y")
                        .Replace("Ruin", "&rRuin&y")
                        .Replace("House Isner", "&MHouse Isner&y");
                }
                else
                {
                    __result = __result
                        .Replace("MASTERWORK", "&cMASTERWORK&y")
                        .Replace("RUIN", "&rRUIN&y")
                        .Replace("HOUSE ISNER", "&MHOUSE ISNER&y");
                }
            }


        }

        static string Capitalize(Match m) => m.Captures[0].Value.ToUpper();
    }
}
