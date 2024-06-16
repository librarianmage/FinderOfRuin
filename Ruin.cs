using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using HarmonyLib;
using XRL;

namespace FinderOfRuin.Patches
{
    [HarmonyPatch(typeof(MarkovChain))]
    [HarmonyPatch(nameof(MarkovChain.AppendSecret))]
    internal static class FormattingPatcher
    {
        private static IEnumerable<CodeInstruction> Transpiler(
            IEnumerable<CodeInstruction> instructions
        )
        {
            var codeMatcher = new CodeMatcher(instructions);

            // replace regex to only accept balanced {}s and match the outermost pair

            codeMatcher.MatchStartForward(new CodeMatch(OpCodes.Ldstr, "{.*?}"));

            if (codeMatcher.IsInvalid)
            {
                UnityEngine.Debug.LogError("FinderOfRuin: Cannot find regex pattern");
                return instructions;
            }

            codeMatcher.SetOperandAndAdvance(LoreFormatter.CluePattern);

            // remove outer {}s from secretNugget (getting

            codeMatcher
                .MatchEndForward(new CodeMatch(instruction => instruction.IsStarg()))
                .Advance(1);

            if (codeMatcher.IsInvalid)
            {
                UnityEngine.Debug.LogError("FinderOfRuin: Cannot find starg anchor");
                return instructions;
            }

            // secretNugget = HeaderMatches.Groups[1].Value;
            codeMatcher.InsertAndAdvance(
                new CodeInstruction[]
                {
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        AccessTools.PropertyGetter(typeof(Match), nameof(Match.Groups))
                    ),
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        AccessTools.Method(
                            typeof(GroupCollection),
                            "get_Item",
                            new[] { typeof(int) }
                        )
                    ),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        AccessTools.PropertyGetter(typeof(Capture), nameof(Capture.Value))
                    ),
                    new CodeInstruction(OpCodes.Stloc_0)
                }
            );

            // prevent stripping of formatting brackets

            codeMatcher
                .MatchStartForward(
                    new CodeMatch(
                        instruction =>
                            instruction.Is(OpCodes.Ldstr, "{") || instruction.Is(OpCodes.Ldstr, "}")
                    ),
                    new CodeMatch(OpCodes.Ldstr, ""),
                    new CodeMatch(
                        instruction =>
                            instruction.Calls(
                                AccessTools.Method(
                                    typeof(string),
                                    nameof(String.Replace),
                                    new[] { typeof(string), typeof(string) }
                                )
                            )
                    )
                )
                .Repeat(codeMatcher => codeMatcher.RemoveInstructions(3));

            return codeMatcher.InstructionEnumeration();
        }
    }

    [HarmonyPatch(typeof(MarkovChain))]
    [HarmonyPatch(nameof(MarkovChain.AppendSecret))]
    internal static class LoreFormatterPatch
    {
        private static void Prefix(
            MarkovChainData Data,
            ref string Secret,
            bool addOpeningWords = false
        ) => Secret = LoreFormatter.FormatLore(Secret);
    }
}
