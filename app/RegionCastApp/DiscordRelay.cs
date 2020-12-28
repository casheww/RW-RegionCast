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

        static async Task RWCheckLoop()
        {
            bool rwIsOpen = CheckRWIsOpen();
            while (rwIsOpen)
            {
                discord.RunCallbacks();
                await Task.Delay(250);
                rwIsOpen = CheckRWIsOpen();
            }

            // clear presence. app will close once this is done
            discord.GetActivityManager().ClearActivity(UpdateActivityCallback);
        }

        static async Task StartListener(int port)
        {
            RCListener client = new RCListener(port);
            await client.Listen();
        }

        static async void OnRCMessage(RCListener client, string rawMessage)
        {
            SetPresence(rawMessage);

            // listen again
            await client.Listen();
        }

        static void SetPresence(string raw)
        {
            Dictionary<string, string> message = Parsing.ParseUdpMessage(raw);
            
            if (!Parsing.ValidateActivityDict(message))
            {
                return;
            }

            Discord.Activity activity = new Discord.Activity();
            activity.Instance = true;

            // === game mode ===
            string mode = message["gamemode"];
            // if gamemode has changed, reset the start timestamp
            if (lastGameMode != mode)
            {
                startTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                lastGameMode = mode;
            }
            activity.State = mode;
            activity.Timestamps = new Discord.ActivityTimestamps { Start = startTimestamp };

            // === location ===
            string location = message["location"];
            location = Parsing.ParseRegionName(message["regioncode"], location);
            activity.Details = location;
            // only update when necessary
            if (lastLocation == location) return;
            else lastLocation = location;

            // === region code ===
            string regionCode = message["regioncode"];
            if (Parsing.ValidRegion(regionCode))
            {
                activity.Assets = new Discord.ActivityAssets { LargeImage = regionCode.ToLower() };
            }
            else
            {
                activity.Assets = new Discord.ActivityAssets { LargeImage = "slugcat" };
            }

            if (int.TryParse(message["playercount"], out int playerCount))
            {
                if (playerCount > 1)
                {
                    activity.State += $" ({playerCount} players)";
                }
            }

            Console.WriteLine($"DiscordRelay.SetPresence : about to update activity : {activity.Details}");
            discord.GetActivityManager().UpdateActivity(activity, UpdateActivityCallback);
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
