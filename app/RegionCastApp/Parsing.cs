using System.Collections.Generic;

namespace RCApp
{
    class Parsing
    {
        static public string GetRegionName(string code)
        {
            /* Tries to return the full region name from the region code.
             * If matching fails, it is either a custom region or an arena and
             * the given code is returned. DiscordRelay.SetPresence has
             * measures for this. */
            Dictionary<string, string> nameDict = new Dictionary<string, string>
            {
                { "CC", "Chimney Canopy" },
                { "DS", "Drainage System" },
                { "GW", "Garbage Wastes" },
                { "HI", "Industrial Complex" },
                { "LF", "Farm Arrays" },
                { "SB", "Subterranean" },
                { "SH", "Shaded Citadel" },
                { "SI", "Sky Islands" },
                { "SL", "Shoreline" },
                { "SS", "Super Structure" },
                { "SU", "Outskirts" },
                { "UW", "The Exterior" },
            };

            if (nameDict.ContainsKey(code))
            {
                return nameDict[code];
            }
            else
            {
                // todo: co-op compat?
                return code;
            }
        }

        static public Dictionary<string, string> ParseUdpMessage(string message)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            string[] lines = message.Split('\n');
            foreach (string field in lines[1..])
            {
                string[] keyValPair = field.Split(':');
                data.Add(keyValPair[0], keyValPair[1]);
            }

            return data;
        }
    }
}
