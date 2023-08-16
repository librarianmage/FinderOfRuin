using System;
using static XRL.UI.Options;

namespace FinderOfRuin
{
    public static class Options
    {
        public static string Option(string name) => $"Books_FinderOfRuin_{name}";

        public static class Formatting
        {
            public static bool Enabled => GetOption(Option("EnableFormatting"), "Yes").EqualsNoCase("Yes");

            public static bool Wanted => Enabled && (Highlight.Enabled || Capitalization.Enabled);

            public static class Highlight
            {
                public static bool Enabled => GetOption(Option("EnableHighlight"), "Yes").EqualsNoCase("Yes");

                public enum HighlightSpan { KeyWords, EntireClue };

                public static HighlightSpan Span {
                    get {
                        var span = GetOption(Option("Highlight"), "Key Words");

                        if (Enum.TryParse<HighlightSpan>(span.Replace(" ", ""), out var result))
                        {
                            return result;
                        }
                        else
                        {
                            return HighlightSpan.KeyWords;
                        }
                    }
                }

                public enum HighlightStyle { AllWhite, ColoredKeyWords };

                public static HighlightStyle Style
                {
                    get
                    {
                        var style = GetOption(Option("HighlightStyle"), "Key Words");

                        if (Enum.TryParse<HighlightStyle>(style.Replace(" ", ""), out var result))
                        {
                            return result;
                        }
                        else
                        {
                            return HighlightStyle.ColoredKeyWords;
                        }
                    }
                }
            }

            public static class Capitalization
            {
                public static bool Enabled => GetOption(Option("EnableCapitalization"), "Yes").EqualsNoCase("Yes");

                public enum CapitalizationSpan { KeyWords, EntireClue };

                public static CapitalizationSpan Span
                {
                    get {
                        var span = GetOption(Option("Highlight"), "Key Words");

                        if (Enum.TryParse<CapitalizationSpan>(span.Replace(" ", ""), out var result))
                        {
                            return result;
                        }
                        else
                        {
                            return CapitalizationSpan.KeyWords;
                        }
                    }
                }
            }
        }

        public static class SecretSmuggler
        {
            public static bool Enabled => GetOption(Option("EnableSmuggling"), "Yes").EqualsNoCase("Yes");
        }
    }
}
