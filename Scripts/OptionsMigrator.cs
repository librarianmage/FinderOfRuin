using XRL;
using static XRL.UI.Options;

namespace FinderOfRuin
{
    [HasModSensitiveStaticCache]
    public static class OptionsMigrator
    {
        [ModSensitiveCacheInit]
        public static void AttemptMigration()
        {
            UnityEngine.Debug.Log("Finder of Ruin: Checking if migration needed…");
            if (
                !(
                    int.TryParse(
                        GetOption("Books_FinderOfRuin_OptionsVersion", "0"),
                        out var version
                    )
                    && version == 1
                )
            )
            {
                UnityEngine.Debug.Log("Finder of Ruin: Migration not done, migrating…");

                if (HasOption("Books_FinderOfRuin_Highlight"))
                {
                    var highlight = GetOption("Books_FinderOfRuin_Highlight");

                    switch (highlight)
                    {
                        case "None":
                            SetOption("Books_FinderOfRuin_EnableHighlight", "No");
                            break;
                        case "Key Words":
                            SetOption("Books_FinderOfRuin_EnableHighlight", "Yes");
                            SetOption("Books_FinderOfRuin_HighlightSpan", "Key Words");
                            break;
                        case "Entire Clue":
                            SetOption("Books_FinderOfRuin_EnableHighlight", "Yes");
                            SetOption("Books_FinderOfRuin_HighlightSpan", "Entire Clue");
                            break;
                        default:
                            break;
                    }
                }

                if (HasOption("Books_FinderOfRuin_HighlightStyle"))
                {
                    var highlightStyle = GetOption("Books_FinderOfRuin_HighlightStyle");

                    if (highlightStyle == "White")
                    {
                        SetOption("Books_FinderOfRuin_HighlightStyle", "Only White");
                    }
                }

                if (HasOption("Books_FinderOfRuin_Capitalization"))
                {
                    var capitalization = GetOption("Books_FinderOfRuin_Capitalization");

                    switch (capitalization)
                    {
                        case "Default":
                            SetOption("Books_FinderOfRuin_EnableUppercasing", "No");
                            break;
                        case "Key Words":
                            SetOption("Books_FinderOfRuin_EnableUppercasing", "Yes");
                            SetOption("Books_FinderOfRuin_UppercasingSpan", "Key Words");
                            break;
                        case "Entire Clue":
                            SetOption("Books_FinderOfRuin_EnableUppercasing", "Yes");
                            SetOption("Books_FinderOfRuin_UppercasingSpan", "Entire Clue");
                            break;
                        default:
                            break;
                    }
                }

                SetOption("Books_FinderOfRuin_OptionsVersion", "1");

                UnityEngine.Debug.Log("Finder of Ruin: Migration done.");
            }
            else
            {
                UnityEngine.Debug.Log("Finder of Ruin: Migration already done");
            }
        }
    }
}
