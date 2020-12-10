using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;

namespace RCApp
{
    class Parsing
    {
        static string jsonURL = "https://casheww.github.io/RW-RegionCast/web/customRegions.json";
        static List<string> supportedCustomRegionCodes = LoadCustomRegions();

        static public bool ValidRegion(string code)
        {
            if (ValidVanillaRegion(code))
            {
                return true;
            }

            return ValidCustomRegion(code); 
        }

        static bool ValidVanillaRegion(string code)
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

        static bool ValidCustomRegion(string code)
        {
            foreach (string r in supportedCustomRegionCodes)
            {
                if (r == code)
                {
                    return true;
                }
            }
            return false;
        }

        static List<string> LoadCustomRegions()
        {
            Dictionary<string, PackDetails> packs;
            List<string> codes = new List<string>();
            using (WebClient client = new WebClient())
            {
                string jsonData = client.DownloadString(jsonURL);
                packs = JsonConvert.DeserializeObject<Dictionary<string, PackDetails>>(jsonData);
            }
            foreach (var pair in packs)
            {
                codes.Add(pair.Value.code);
            }
            System.Console.WriteLine(codes[0]);
            return codes;
        }

        struct PackDetails
        {
            public List<string> authors;
            public string code;
            public string url;
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
