using static XRL.UI.Options;

namespace FinderOfRuin
{
    public static class Options
    {
        private static string Option(string Name) => $"Books_FinderOfRuin_{Name}";

        public static class Formatting
        {
            public static bool Enabled =>
                GetOption(Option("EnableFormatting"), "Yes").EqualsNoCase("Yes");

            public static bool Wanted => Enabled && (Highlight.Enabled || Capitalization.Enabled);

            public static bool WantKeyWords =>
                Enabled && (Highlight.WantKeyWords || Capitalization.WantKeyWords);

            public static bool WantEntireClue =>
                Enabled && (Highlight.WantEntireClue || Capitalization.WantEntireClue);

            public enum Span
            {
                KeyWords,
                EntireClue
            };

            public static class Highlight
            {
                public static bool Enabled =>
                    GetOption(Option("EnableHighlight"), "Yes").EqualsNoCase("Yes");

                public static Span Span =>
                    GetOption(Option("HighlightSpan")) switch
                    {
                        "Key Words" => Span.KeyWords,
                        "Entire Clue" => Span.EntireClue,
                        _ => Span.KeyWords
                    };

                public enum HighlightStyle
                {
                    AllWhite,
                    ColoredKeyWords
                };

                public static HighlightStyle Style =>
                    GetOption(Option("HighlightStyle")) switch
                    {
                        "All White" => HighlightStyle.AllWhite,
                        "Colored Key Words" => HighlightStyle.ColoredKeyWords,
                        _ => HighlightStyle.ColoredKeyWords
                    };

                public static bool WantKeyWords =>
                    Enabled && (Span == Span.KeyWords || Style == HighlightStyle.ColoredKeyWords);

                public static bool WantEntireClue => Enabled && Span == Span.EntireClue;
            }

            public static class Capitalization
            {
                public static bool Enabled =>
                    GetOption(Option("EnableCapitalization"), "Yes").EqualsNoCase("Yes");

                public static Span Span =>
                    GetOption(Option("CapitalizationSpan")) switch
                    {
                        "Key Words" => Span.KeyWords,
                        "Entire Clue" => Span.EntireClue,
                        _ => Span.KeyWords
                    };

                public static bool WantKeyWords => Enabled && Span == Span.KeyWords;

                public static bool WantEntireClue => Enabled && Span == Span.EntireClue;
            }
        }
    }
}
