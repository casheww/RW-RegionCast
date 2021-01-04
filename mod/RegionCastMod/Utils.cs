using System.Collections.Generic;

namespace RegionCast
{
    class Utils
    {
        public static string GetSlugName(string rawName)
        {
            switch (rawName)
            {
                case "Yellow":
                    return "Monk";

                case "White":
                    return "Survivor";

                case "Red":
                    return "Hunter";

                default:
                    return rawName;
            }
        }

        static List<string> nonSlugs = new List<string>
        {
            "Menu", "Dead", "Dreaming", "Sleeping", "Arena"
        };

        public static string GameModeAppend(string gameMode)
        {
            switch (ConfigMenu.AppendSetting)
            {
                case ConfigMenu.Append.Cycles:
                    if (!nonSlugs.Contains(gameMode))
                    {
                        return $" (cycle {RegionCast.CycleNumber})";
                    }
                    return "";
                        
                case ConfigMenu.Append.Players:
                    if (!nonSlugs.Contains(gameMode) && RegionCast.PlayerCount > 1)
                    {
                        return $" ({RegionCast.PlayerCount} players)";
                    }
                    return "";
                        

                default:
                case ConfigMenu.Append.None:
                    return "";
            }
        }
    }
}
