using ConsoleLib.Console;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using XRL;

namespace FinderOfRuin.Patches
{
    [HarmonyPatch(typeof(MarkovChain))]
    [HarmonyPatch(nameof(MarkovChain.AppendSecret))]
    static class FormattingPatcher
    {
        public static string SecretRegex = @"\{((?:[^\{\}]|(?<open>\{)|(?<-open>\}))+(?(open)(?!)))\}";

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var code = new List<CodeInstruction>(instructions);

            // Replace regex to only accept balanced {}s
            var regexidx = code.FindIndex(x => x.Is(OpCodes.Ldstr, "{.*?}"));

            if (regexidx == -1)
            {
                UnityEngine.Debug.Log("Regex not found!");
                return instructions;
            }

            code[regexidx].operand = SecretRegex;

            // Remove outer {}s from secretNugget

            var stargsidx = code.FindIndex(x => x.IsStarg());

            if (stargsidx == -1)
            {
                UnityEngine.Debug.Log("starg not found!");
                return instructions;
            }

            code.InsertRange(stargsidx + 1, new List<CodeInstruction> {
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Match), nameof(Match.Groups))),
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(GroupCollection), "get_Item", new[] { typeof(int) })),
                    new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Capture), nameof(Capture.Value))),
                    new CodeInstruction(OpCodes.Stloc_0)
                });


            // Prevent stripping of {}s

            var stripidxs =
                code
                .Select((x, i) => new { x, i })
                .Where(z =>
                       (z.x.Is(OpCodes.Ldstr, "{")
                        || z.x.Is(OpCodes.Ldstr, "}"))
                       && z.i + 2 < code.Count
                       && code[z.i + 1].Is(OpCodes.Ldstr, "")
                       && code[z.i + 2].Calls(AccessTools.Method(typeof(String), nameof(String.Replace), new[] { typeof(String), typeof(string) }))
                )
                .Select(z => z.i).ToList();


            if (stripidxs.Count == 0)
            {
                UnityEngine.Debug.Log("stripping not found!");
                return instructions;
            }

            stripidxs.Reverse();

            foreach (var idx in stripidxs)
            {
                code.RemoveRange(idx, 3);
            }

            return code;
        }
    }

    [HarmonyPatch(typeof(LoreGenerator))]
    [HarmonyPatch(nameof(LoreGenerator.RuinOfHouseIsnerLore))]
    static class LorePatcher
    {
        static void Postfix(ref string __result)
        {
            FormatSecret(ref __result);
        }

        public static void FormatSecret(ref string __result)
        {
            if (!Options.Formatting.Enabled) return;

            Match matches = Regex.Match(__result, FormattingPatcher.SecretRegex);

            UnityEngine.Debug.Log(matches.Groups[1].Value);

            __result = __result.Replace(matches.Groups[1].Value, Markup.Color("W", matches.Groups[1].Value));

            // String Highlight = Options.GetOption("Books_FinderOfRuin_Highlight", "Key Words");
            // String HighlightStyle = Options.GetOption("Books_FinderOfRuin_HighlightStyle", "Colored Key Words");
            // String Capitalization = Options.GetOption("Books_FinderOfRuin_Capitalization", "Default");

            // if (Highlight == "Entire Clue" || Capitalization == "Entire Clue")
            // {
            //     Regex rx = new Regex(@"\{(.*)\}");

            //     Func<Match, String> match = (Match m) =>
            //     {
            //         String hint = m.Captures[0].Value;
            //         if (Capitalization == "Entire Clue") { hint = hint.ToUpper(); };
            //         if (Highlight == "Entire Clue") { hint = "&Y" + hint + "&y"; }
            //         return hint;
            //     };

            //     MatchEvaluator evaluator = new MatchEvaluator(match);
            //     __result = rx.Replace(__result, evaluator);
            // }

            // if (Highlight == "Key Words" || Capitalization == "Key Words" || (Highlight == "Entire Clue" && HighlightStyle == "Colored Key Words"))
            // {
            //     Regex rx = new Regex(@"(masterwork|ruin|house isner)", RegexOptions.IgnoreCase);

            //     Func<Match, String> match = (Match m) =>
            //     {
            //         String hint = m.Captures[0].Value;
            //         if (Capitalization == "Key Words") { hint = hint.ToUpper(); };
            //         if (Highlight != "Default" && HighlightStyle == "Colored Key Words")
            //         {
            //             String restColor = (Highlight == "Entire Clue") ? "&Y" : "&y";
            //             String startColor;

            //             if (hint.ToLower() == "masterwork")
            //             {
            //                 startColor = "&c";
            //             }
            //             else if (hint.ToLower() == "ruin")
            //             {
            //                 startColor = "&r";
            //             }
            //             else if (hint.ToLower() == "house isner")
            //             {
            //                 startColor = "&M";
            //             }
            //             else
            //             {
            //                 startColor = restColor;
            //             }

            //             hint = startColor + hint + restColor;
            //         }
            //         else if (Highlight == "Key Words" && HighlightStyle == "White")
            //         {
            //             hint = "&Y" + hint + "&y";
            //         }
            //         return hint;
            //     };

            //     MatchEvaluator evaluator = new MatchEvaluator(match);
            //     __result = rx.Replace(__result, evaluator);
            // }
        }
    }
}
