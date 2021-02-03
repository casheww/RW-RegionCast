using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace RCApp
{
    class Parsing
    {
        static string jsonURL = "https://casheww.github.io/RW-RegionCast/web/customRegions.json";
        static List<string> thumbedCustomRegions = LoadCustomRegionThumbnails();
        static string[] regionPacks = LoadRegionPacks();

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
            foreach (string r in thumbedCustomRegions)
            {
                if (r == code)
                {
                    return true;
                }
            }
            return false;
        }

        static List<string> LoadCustomRegionThumbnails()
        {
            Dictionary<string, PackDetails> packs;
            List<string> codes = new List<string>();
            using (WebClient client = new WebClient())
            {
                string jsonData = client.DownloadString(jsonURL);
                packs = JsonConvert.DeserializeObject<Dictionary<string, PackDetails>>(jsonData);
            }
            string logStr = "Region packs with custom thumbnail support:\n";
            foreach (var pair in packs)
            {
                codes.Add(pair.Value.code);
                logStr += $"{pair.Key} : {pair.Value.code} : {pair.Value.url}\n";
            }
            DiscordRelay.Log(logStr);
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

        static readonly string[] messageKeys =
        {
            "gamemode", "location", "regioncode"
        };

        static public bool ValidateActivityDict(Dictionary<string, string> dict)
        {
            foreach (string key in messageKeys)
            {
                if (!dict.ContainsKey(key))
                {
                    return false;
                }
            }
            return true;
        }

        static string[] LoadRegionPacks()
        {
            string CRResourcesPath = Directory.GetCurrentDirectory() +
                Path.DirectorySeparatorChar + "Mods" +
                Path.DirectorySeparatorChar + "CustomResources";

            string[] regionPacks;
            try
            {
                regionPacks = Directory.GetDirectories(CRResourcesPath);
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("CustomResources dir not found");
                regionPacks = new string[] { };
            }

            return regionPacks;
        }

        static public string ParseRegionName(string regionCode, string origName)
        {
            string newName;

            // read replacements from custom region files
            newName = GetCustomRegionNameOverwrite(regionCode, origName);

            // remove vanilla spoilers
            newName = newName.Replace("Looks to the Moon", "LttM");
            newName = newName.Replace("Five Pebbles", "5P");

            return newName;
        }

        static string GetCustomRegionNameOverwrite(string code, string inputName)
        {
            // iterate through regions in each pack
            foreach (string pack in regionPacks)
            {
                string packRegionsPath = pack +
                        Path.DirectorySeparatorChar + "World" +
                        Path.DirectorySeparatorChar + "Regions";

                if (Directory.Exists(Path.Combine(packRegionsPath, code)))
                {
                    string configPath = Path.Combine(packRegionsPath, code, "RegionCast.txt");
                    if (File.Exists(configPath))
                    {
                        string[] lines = File.ReadAllLines(configPath);
                        foreach (string l in lines)
                        {
                            if (l.StartsWith(inputName))
                            {
                                return l.Split(':')[1];
                            }
                        }
                    }
                }
            }
            return inputName;
        }

    }
}
