using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace RCApp
{
    class DiscordRelay
    {
        // discord
        static readonly Discord.Discord discord = new Discord.Discord(746839575124770917, (long)Discord.CreateFlags.NoRequireDiscord);

        // mod data cache
        static long startTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        static string lastGameMode = "";
        static string lastLocation = "none";

        // paths
        static readonly string thisDirPath = Path.Combine(Directory.GetCurrentDirectory(), "RegionCast-DiscordGameSDK");
        static readonly string logPath = Path.Combine(thisDirPath, "exception.log");
        static readonly string configPath = Path.Combine(thisDirPath, "config.txt");

        static async Task Main()
        {
            AppDomain.CurrentDomain.UnhandledException += ExceptionLogger;
            RCListener.MessageReceived += OnRCMessage;

            // load config
            string[] config = File.ReadAllLines(configPath);
            bool validPort = int.TryParse(config[0], out int port);
            if (!validPort) { return; }

            await Task.WhenAll(StartListener(port), RWCheckLoop());
        }

        static async Task StartListener(int port)
        {
            RCListener client = new RCListener(port);
            await client.Listen();
        }

        static async void OnRCMessage(RCListener client, Dictionary<string, string> message)
        {
            SetPresence(message);

            // listen again
            await client.Listen();
        }

        static async Task RWCheckLoop()
        {
            bool rwIsOpen = CheckRWIsOpen();
            while (rwIsOpen)
            {
                discord.RunCallbacks();
                await Task.Delay(100);
                rwIsOpen = CheckRWIsOpen();
            }

            // clear presence. app will close once this is done
            discord.GetActivityManager().ClearActivity(UpdateActivityCallback);
        }

        static void SetPresence(Dictionary<string, string> message)
        {
            Discord.Activity activity = new Discord.Activity();
            activity.Instance = true;

            // set gamemode
            if (message.ContainsKey("gamemode"))
            {
                activity.State = message["gamemode"];

                // if gamemode has changed, reset the start timestamp
                if (lastGameMode != message["gamemode"])
                {
                    startTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    lastGameMode = message["gamemode"];
                }
            }
            activity.Timestamps = new Discord.ActivityTimestamps { Start = startTimestamp };

            // set location name
            if (message.ContainsKey("location"))
            {
                string location = message["location"];

                if (message.ContainsKey("regioncode"))
                {
                    location = Parsing.ParseRegionName(message["regioncode"], location);
                }
                
                activity.Details = location;

                // only update when necessary. avoids hitting rate limits (5 per 20 seconds) by a lot
                if (lastLocation == location)
                {
                    return;
                }
                else
                {
                    lastLocation = location;
                }
            }

            // thumbnail and fallback location name
            if (message.ContainsKey("regioncode"))
            {
                string code = message["regioncode"];

                if (Parsing.ValidRegion(code))
                {
                    activity.Assets = new Discord.ActivityAssets { LargeImage = code.ToLower() };
                }
                else
                {
                    activity.Assets = new Discord.ActivityAssets { LargeImage = "slugcat" };
                }

            }

            // set playercount if it's not 0 (singleplayer)
            if (message.ContainsKey("playercount"))
            {
                if (message["playercount"] != "0")
                {
                    activity.State += $" ({message["playercount"]} of 4)";
                }
            }

            Console.WriteLine($"DiscordRelay.SetPresence : about to update activity : {activity.Details}");
            Discord.ActivityManager activityManager = discord.GetActivityManager();

            activityManager.UpdateActivity(activity, UpdateActivityCallback);
        }

        static void UpdateActivityCallback(Discord.Result res)
        {
            Console.WriteLine(res);
        }

        static bool CheckRWIsOpen()
        {
            Process[] RWProcesses = Process.GetProcessesByName("RainWorld");
            if (RWProcesses.Length < 1)
            {
                return false;
            }
            return true;
        }

        private static void ExceptionLogger(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = e.ExceptionObject as Exception;
            if (exception is null) { return; }

            using (StreamWriter sw = File.CreateText(logPath))
            {
                sw.Write(exception.ToString());
            }
        }
    }
}
