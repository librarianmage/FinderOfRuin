using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using ConsoleLib.Console;
using static FinderOfRuin.Options.Formatting;

namespace FinderOfRuin
{
    /// <summary>
    ///   Utilities for formatting Isner clues.
    /// </summary>
    public static class LoreFormatter
    {
        /// <remarks>
        ///   Inspired by <see href="https://stackoverflow.com/a/7899205">Scott Rippey's StackOverflow answer</see>.
        /// </remarks>
        private const string MatchingCurlyBraces =
            @"(?:[^\{\}]|(?<open>\{)|(?<-open>\}))+(?(open)(?!))";

        /// <remarks>
        ///   Made public for <see cref="Patches.FormattingPatcher"/>.
        /// </remarks>
        public static readonly string CluePattern = $@"\{{({MatchingCurlyBraces})\}}";

        /// <summary>
        ///   Matches the entire clue fragment. The first group is the inner clue text.
        /// </summary>
        public static readonly Regex ClueRegex = new(CluePattern);

        private static readonly string ClueInnerPattern = $@"(?<=\{{){MatchingCurlyBraces}(?=\}})";

        /// <summary>
        ///   Matches the inner clue text, without the braces.
        /// </summary>
        public static readonly Regex ClueInnerRegex = new(ClueInnerPattern);

        /// <summary>
        ///   Formats the entire clue based on <see cref="Options.Formatting" />.
        /// </summary>
        /// <remarks>
        ///   This function gets used as a <see cref="MatchEvaluator" />.
        /// </remarks>
        /// <returns>
        ///   The formatted match.
        /// </returns>
        /// <param name="ClueMatch">
        ///   A <see cref="Match"/> of the inside of a clue.
        /// </param>
        public static string FormatClueInner(Match ClueMatch)
        {
            var clue = ClueMatch.Value;

            if (!WantEntireClue)
            {
                return clue;
            }

            if (Highlight.WantEntireClue)
            {
                clue = Highlight.Style switch
                {
                    Highlight.HighlightStyle.OnlyWhite => Markup.Color("Y", clue),
                    Highlight.HighlightStyle.ColoredKeyWords => Markup.Color("Y", clue),
                    _
                        => throw new ArgumentOutOfRangeException(
                            nameof(Highlight.HighlightStyle),
                            $"Finder of Ruin: Unexpected highlight style: {Highlight.Style}"
                        )
                };
            }

            if (Uppercasing.WantEntireClue)
            {
                clue = ColorUtility.CapitalizeExceptFormatting(clue);
            }

            return clue;
        }

        private const string KeyWordPattern = @"(?:masterwork)|(?:Ruin)|(?:House Isner)";

        /// <summary>
        ///   Matches the notable key words.
        /// </summary>
        public static readonly Regex KeyWordRegex = new(KeyWordPattern);

        /// <summary>
        ///   Map of key word to color format.
        /// </summary>
        public static readonly ReadOnlyDictionary<string, string> KeyWordColors =
            new(
                new Dictionary<string, string>
                {
                    { "masterwork", "c" },
                    { "Ruin", "r" },
                    { "House Isner", "M" }
                }
            );

        /// <summary>
        ///   Formats the key word matches based on <see cref="Options.Formatting" />.
        /// </summary>
        /// <remarks>
        ///   This function gets used as a <see cref="MatchEvaluator" />.
        /// </remarks>
        /// <returns>
        ///   The formatted match.
        /// </returns>
        /// <param name="KeyWordMatch">
        ///   A <see cref="Match"/> of a key word.
        /// </param>
        public static string FormatKeyWords(Match KeyWordMatch)
        {
            var keyWord = KeyWordMatch.Value;

            if (!WantKeyWords)
            {
                return keyWord;
            }

            if (Highlight.WantKeyWords)
            {
                keyWord = Highlight.Style switch
                {
                    Highlight.HighlightStyle.OnlyWhite => Markup.Color("Y", keyWord),
                    Highlight.HighlightStyle.ColoredKeyWords
                        => KeyWordColors.TryGetValue(keyWord, out var color)
                            ? Markup.Color(color, keyWord)
                            : keyWord,
                    _
                        => throw new ArgumentOutOfRangeException(
                            nameof(Highlight.HighlightStyle),
                            $"Finder of Ruin: Unexpected highlight style: {Highlight.Style}"
                        )
                };
            }

            if (Uppercasing.WantKeyWords)
            {
                keyWord = ColorUtility.ToUpperExceptFormatting(keyWord);
            }

            return keyWord;
        }

        ///<summary>Formats an Isner clue.</summary>
        ///<remarks>For use in <see cref="Patches.LoreFormatterPatch"/>.</remarks>
        public static string FormatLore(string Lore)
        {
            if (!Wanted)
            {
                return Lore;
            }

            if (WantKeyWords)
            {
                var keyWordMatcher = new MatchEvaluator(FormatKeyWords);
                Lore = KeyWordRegex.Replace(Lore, keyWordMatcher);
            }

            if (WantEntireClue)
            {
                var clueMatcher = new MatchEvaluator(FormatClueInner);
                Lore = ClueInnerRegex.Replace(Lore, clueMatcher);
            }

            return Lore;
        }
    }
}
