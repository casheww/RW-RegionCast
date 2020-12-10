using System.Collections.Generic;

namespace RCApp
{
    class Parsing
    {
        static public bool ValidCodeForThumbnail(string code)
        {
            string[] regions = { "CC", "DS", "GW", "HI", "LF", "SB", "SH", "SI", "SL", "SS", "SU", "UW" };
            foreach (string r in regions)
            {
                if (r == code)
                {
                    return true;
                }
            }
            return false;
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
