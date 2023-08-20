using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using ConsoleLib.Console;
using HarmonyLib;
using XRL;
using XRL.Wish;
using XRL.World.Parts;

namespace FinderOfRuin.Patches
{
    static class Secret
    {
        public static string Name(int i) => $"FinderOfRuin Secret {i}";

        public static bool Has(int i) => The.Game.HasStringGameState(Name(i));

        //public static string GetSecret(int i)
    }

    [HasWishCommand]
    [HarmonyPatch(typeof(MarkovBook))]
    [HarmonyPatch(nameof(MarkovBook.EnsureCorpusLoaded))]
    static class SecretSmuggler
    {
        static bool Applied = false;

        static string SecretName(int i) => $"FinderOfRuin Secret {i}";

        static bool HasSecret(int i) => The.Game.HasStringGameState(SecretName(i));
        static string GetSecret(int i) => The.Game.GetStringGameState(SecretName(i), "NO SECRET");
        static void StoreSecret(int i, string secret) => The.Game.SetStringGameState(SecretName( i), secret);

        struct SmugglerState
        {
            public string Corpus;
            public bool OldGame;
        }

        static void Prefix(string Corpus, out SmugglerState __state)
        {
            __state = new SmugglerState {
                Corpus = Corpus,
                OldGame = The.Game?.HasIntGameState("AddedMarkovSecrets") ?? false
            };
        }

        static void Postfix(SmugglerState __state)
        {
            bool OldGame = __state.OldGame;
            string Corpus = __state.Corpus;

            if (OldGame && !Applied)
            {
                Applied = true;
                // Reload secrets
                if (MarkovBook.CorpusData.TryGetValue(Corpus, out var data))
                {
                    if (HasSecret(0))
                    {
                        UnityEngine.Debug.Log("Adding secrets back!");
                        for (int i = 0; i < MarkovBook.NUMBER_ISNER_SECRETS; i++)
                        {
                            string secret = GetSecret(i);

                            MarkovChain.AppendSecret(data, secret);
                        }
                    }
                    else
                    {
                        UnityEngine.Debug.LogError("Missing secrets!");
                    }
                }
                else
                {
                    UnityEngine.Debug.LogError($"Corpus {Corpus} not loaded");
                }
            }
            else
            {
                Applied = true;
            }
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var code = new List<CodeInstruction>(instructions);

            var callidx = code.FindIndex(x => x.Is(OpCodes.Call, AccessTools.Method(typeof(LoreGenerator), nameof(LoreGenerator.RuinOfHouseIsnerLore))));

            if (callidx == -1)
            {
                UnityEngine.Debug.Log("Instruction not found!");
                return code;
            }

            code[callidx].operand = AccessTools.Method(typeof(SecretSmuggler), nameof(SecretSmuggler.SmuggleSecret));

            code.Insert(callidx, new CodeInstruction(OpCodes.Ldloc_1));

            return code;
        }

        static string SmuggleSecret(int x, int y, int i)
        {
            string secret = LoreGenerator.RuinOfHouseIsnerLore(x, y);
            StoreSecret(i, ColorUtility.StripFormatting(secret));
            return secret;
        }

        [WishCommand("revealisnersecrets")]
        static void RevealSecrets()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < MarkovBook.NUMBER_ISNER_SECRETS; i++)
            {
                sb.Append($"{i}. ");
                sb.AppendLine(GetSecret(i));
            }

            XRL.UI.Popup.Show(sb.ToString(), LogMessage: false);
        }
    }
}
