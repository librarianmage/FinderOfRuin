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

            code[regexidx].operand = LoreFormatter.CluePattern;

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


            // if (stripidxs.Count == 0)
            // {
            //     UnityEngine.Debug.Log("stripping not found!");
            //     return code; // not essential to functioning
            // }

            stripidxs.Reverse();

            foreach (var idx in stripidxs)
            {
                code.RemoveRange(idx, 3);
            }

            return code;
        }
    }

    [HarmonyPatch(typeof(MarkovChain))]
    [HarmonyPatch(nameof(MarkovChain.AppendSecret))]
    static class LoreFormatterPatch
    {
        static void Prefix(MarkovChainData Data, ref string Secret, bool addOpeningWords = false)
        {
            Secret = LoreFormatter.FormatLore(Secret);
        }
    }
}
