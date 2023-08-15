using HarmonyLib;
using System;
using System.Text.RegularExpressions;
using XRL;
using XRL.UI;

namespace FinderOfRuin.Patches
{
    [HarmonyPatch(typeof(LoreGenerator))]
    [HarmonyPatch(nameof(LoreGenerator.RuinOfHouseIsnerLore))]
    class LorePatcher
    {
        static void Postfix(ref string __result)
        {
            FormatSecret(ref __result);
        }

        public static void FormatSecret(ref string __result)
        {
            String Highlight = Options.GetOption("Books_FinderOfRuin_Highlight", "Key Words");
            String HighlightStyle = Options.GetOption("Books_FinderOfRuin_HighlightStyle", "Colored Key Words");
            String Capitalization = Options.GetOption("Books_FinderOfRuin_Capitalization", "Default");

            if (Highlight == "Entire Clue" || Capitalization == "Entire Clue")
            {
                Regex rx = new Regex(@"\{(.*)\}");

                Func<Match, String> match = (Match m) =>
                {
                    String hint = m.Captures[0].Value;
                    if (Capitalization == "Entire Clue") { hint = hint.ToUpper(); };
                    if (Highlight == "Entire Clue") { hint = "&Y" + hint + "&y"; }
                    return hint;
                };

                MatchEvaluator evaluator = new MatchEvaluator(match);
                __result = rx.Replace(__result, evaluator);
            }

            if (Highlight == "Key Words" || Capitalization == "Key Words" || (Highlight == "Entire Clue" && HighlightStyle == "Colored Key Words"))
            {
                Regex rx = new Regex(@"(masterwork|ruin|house isner)", RegexOptions.IgnoreCase);

                Func<Match, String> match = (Match m) =>
                {
                    String hint = m.Captures[0].Value;
                    if (Capitalization == "Key Words") { hint = hint.ToUpper(); };
                    if (Highlight != "Default" && HighlightStyle == "Colored Key Words")
                    {
                        String restColor = (Highlight == "Entire Clue") ? "&Y" : "&y";
                        String startColor;

                        if (hint.ToLower() == "masterwork")
                        {
                            startColor = "&c";
                        }
                        else if (hint.ToLower() == "ruin")
                        {
                            startColor = "&r";
                        }
                        else if (hint.ToLower() == "house isner")
                        {
                            startColor = "&M";
                        }
                        else
                        {
                            startColor = restColor;
                        }

                        hint = startColor + hint + restColor;
                    }
                    else if (Highlight == "Key Words" && HighlightStyle == "White")
                    {
                        hint = "&Y" + hint + "&y";
                    }
                    return hint;
                };

                MatchEvaluator evaluator = new MatchEvaluator(match);
                __result = rx.Replace(__result, evaluator);
            }
        }
    }
}
