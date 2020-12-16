using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace RCApp
{
    class DiscordRelay
    {
        static Discord.Discord discord = new Discord.Discord(746839575124770917, (long)Discord.CreateFlags.NoRequireDiscord);

        static long startTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        static string lastGameMode = "";
        static string lastLocation = "none";

        static bool messageReceived = false;
        static Dictionary<string, string> message;

        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += ExceptionLogger;

            string configPath = Directory.GetCurrentDirectory() +
                Path.DirectorySeparatorChar + "RegionCast-DiscordGameSDK" +
                Path.DirectorySeparatorChar + "config.txt";
            string[] config = File.ReadAllLines(configPath);

            bool validPort = int.TryParse(config[0], out int port);
            if (!validPort) { return; }

            UdpClient udp = new UdpClient(port);
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);

            RunReceiveLoop(udp, endpoint);
        }

        private static void ExceptionLogger(object sender, UnhandledExceptionEventArgs e)
        {
            string path = Directory.GetCurrentDirectory() +
                Path.DirectorySeparatorChar + "RegionCast-DiscordGameSDK" +
                Path.DirectorySeparatorChar + "exception.log";
            Exception exception = e.ExceptionObject as Exception;
            if (exception is null) { return; }

            using (StreamWriter sw = File.CreateText(path))
            {
                sw.Write(exception.ToString());
            }            
        }

        static void RunReceiveLoop(UdpClient udp, IPEndPoint endpoint)
        {
            // state to pass to async receiver
            UdpState state = new UdpState
            {
                udp = udp,
                endpoint = endpoint
            };

            bool rwIsOpen = CheckRWIsOpen();
            bool lastRunReceivedMessage = false; bool firstRun = true;
            while (rwIsOpen)
            {
                if (lastRunReceivedMessage || firstRun)
                {
                    // start async receiver
                    udp.BeginReceive(new AsyncCallback(RunReceiver), state);
                }
                firstRun = false;

                rwIsOpen = CheckRWIsOpen();

                if (messageReceived)
                {
                    SetPresence();
                    messageReceived = false;
                    lastRunReceivedMessage = true;
                }
                else
                {
                    lastRunReceivedMessage = false;
                }
                discord.RunCallbacks();
            }

            // close
            discord.GetActivityManager().ClearActivity(UpdateActivityCallback);
        }

        static void RunReceiver(IAsyncResult result)
        {
            UdpClient udp = ((UdpState)result.AsyncState).udp;
            IPEndPoint endpoint = ((UdpState)result.AsyncState).endpoint;

            byte[] bytes = udp.EndReceive(result, ref endpoint);
            string rawMessage = Encoding.UTF8.GetString(bytes);
            Console.WriteLine("message receieved from mod");

            if (rawMessage.StartsWith("rwRegionCastData"))
            {
                // custom parsing of UDP from mod
                message = Parsing.ParseUdpMessage(rawMessage);
                messageReceived = true;
            }
        }

        static void SetPresence()
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

                // remove vanilla spoilers
                location = location.Replace("Looks to the Moon", "LttM");
                location = location.Replace("Five Pebbles", "5P");

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

            Console.WriteLine($"about to update activity : {activity.Details}");
            Discord.ActivityManager activityManager = discord.GetActivityManager();

            activityManager.UpdateActivity(activity, UpdateActivityCallback);
        }

        static void UpdateActivityCallback(Discord.Result res)
        {
            Console.WriteLine(res);
        }

        struct UdpState
        {
            public UdpClient udp;
            public IPEndPoint endpoint;
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
    }
}
