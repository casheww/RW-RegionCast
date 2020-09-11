using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Collections.Generic;

namespace RegionCastApp
{
    class RichPresSetter
    {
        static void Main()
        {
            StartListener();
        }

        public static void StartListener()
        {
            var discord = new Discord.Discord(746839575124770917, (UInt64)Discord.CreateFlags.Default);
            var activityManager = discord.GetActivityManager();

            int port = 49181;
            var listener = new UdpClient(port);
            var endpoint = new IPEndPoint(IPAddress.Any, port);

            long startTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            string lastLocation = "Menu";

            try
            {
                while (true)
                {
                    CheckRWIsOpen();

                    byte[] bytes = listener.Receive(ref endpoint);
                    string currentLocation = Encoding.ASCII.GetString(bytes);

                    if (currentLocation != lastLocation)
                    {
                        lastLocation = currentLocation;
                        UpdateLocation(currentLocation);
                    }

                    discord.RunCallbacks();
                }
            }
            catch (Exception e) { Console.WriteLine(e); }
            finally { listener.Close(); }

            void UpdateLocation(string location)
            {
                string[] codes = GetAssetCodes(location);
                var activity = new Discord.Activity
                {
                    State = codes[0],
                    Assets = { LargeImage = codes[1] },
                    Timestamps = { Start = startTime },
                    Instance = true
                };
                activityManager.UpdateActivity(activity, (res) => { Console.WriteLine($"== {res}"); });
            }
        }

        public static string[] GetAssetCodes(string regionCode)
        {
            var dict = new Dictionary<string, string[]>
            {
                { "CC", new string[] {"Chimney Canopy", "cc" } },
                { "DS", new string[] {"Drainage System", "ds" } },
                { "GW", new string[] {"Garbage Wastes", "gw" } },
                { "HI", new string[] {"Industrial Complex", "hi" } },
                { "LF", new string[] {"Farm Arrays", "lf" } },
                { "SB", new string[] {"Subterranean", "sb" } },
                { "SH", new string[] {"Shaded Citadel", "sh" } },
                { "SI", new string[] {"Sky Islands", "si" } },
                { "SL", new string[] {"Shoreline", "sl" } },
                { "SS", new string[] {"Super Structure", "ss" } },
                { "SU", new string[] {"Outskirts", "su" } },
                { "UW", new string[] {"The Exterior", "uw" } },
            };

            if (dict.ContainsKey(regionCode))
            {
                return dict[regionCode];
            }
            else
            {
                return new string[] { regionCode, "slugcat" };
            }
        }

        public static void CheckRWIsOpen()
        {
            var RWProcesses = Process.GetProcessesByName("RainWorld");
            if (RWProcesses.Length < 1)
            {
                Environment.Exit(0);
            }
        }
    }
}
