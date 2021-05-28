using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace RCApp
{
    class Program
    {
        // discord
        static readonly Discord.Discord discord = new Discord.Discord(746839575124770917, (long)Discord.CreateFlags.NoRequireDiscord);
        static object discordLock = new object();
        static Discord.ActivityManager.ClearActivityHandler clearHandler = (res) => { Console.WriteLine(res); };
        static Discord.ActivityManager.UpdateActivityHandler updateHandler = (res) => { Console.WriteLine(res); };

        // mod data cache
        static long startTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        static string lastGameMode = "";
        static string lastLocation = "none";

        // paths
        static readonly string thisDirPath = Path.Combine(Directory.GetCurrentDirectory(), "RegionCast-DiscordGameSDK");
        static readonly string exceptionLogPath = Path.Combine(thisDirPath, "eLog.txt");
        static readonly string logPath = Path.Combine(thisDirPath, "log.txt");
        static readonly string configPath = Path.Combine(thisDirPath, "config.txt");

        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += ExceptionLogger;
            RCListener.MessageReceived += OnRCMessage;

            // set log files
            File.WriteAllText(exceptionLogPath, "");
            File.WriteAllText(logPath, "");
            Log("Starting RegionCast sideapp\nWelcome to modders and the curious...\n");

            // load config
            string[] config = File.ReadAllLines(configPath);
            bool validPort = int.TryParse(config[0], out int port);
            Log($"Port : {port}");
            if (!validPort) return;

            RunThreads(port);

            // the program will now end and the background listener thread will be stopped
            Log("Exit, persued by bear");
        }

        static void RunThreads(int port)
        {
            Thread rwCheckThread = new Thread(new ThreadStart(RWCheckLoop)) { IsBackground = true } ;
            rwCheckThread.Start();

            Thread listenerThread = new Thread(() => StartListener(port)) { IsBackground = true };
            listenerThread.Start();

            rwCheckThread.Join();

            // at this point rain world is closed and the check thread has completed
            lock (discordLock)
            {
                discord.GetActivityManager().ClearActivity(clearHandler);
            }
        }

        static void RWCheckLoop()
        {
            bool rwIsOpen;
            do
            {
                try
                {
                    lock (discordLock)
                    {
                        discord.RunCallbacks();
                    }
                }
                catch (NullReferenceException) { Console.WriteLine("Discord threw nullref"); }

                Thread.Sleep(500);
                rwIsOpen = CheckRWIsOpen();
            } while (rwIsOpen);

            Log("Rain World closed - self-destruct in T minus a very small time...");
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

        static void StartListener(int port)
        {
            RCListener client = new RCListener(port);
            client.Listen();
        }

        static void OnRCMessage(RCListener client, string rawMessage)
        {
            SetPresence(rawMessage);

            // listen again
            client.Listen();
        }

        static void SetPresence(string raw)
        {
            Dictionary<string, string> message = Parsing.ParseUdpMessage(raw);

            if (!Parsing.ValidateActivityDict(message))
            {
                return;
            }

            Discord.Activity activity = new Discord.Activity { Instance = true };

            string mode = message["gamemode"];
            string location = message["location"];
            string regionCode = message["regioncode"];

            if (lastLocation == location && mode == lastGameMode)
            {
                return;
            }
            else lastLocation = location;

            // if gamemode has changed (and neither before nor after is "Dead"), reset the start timestamp
            if (lastGameMode != mode && lastGameMode != "Dead" && mode != "Dead")
            {
                startTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                lastGameMode = mode;
            }
            activity.State = mode;
            activity.Timestamps = new Discord.ActivityTimestamps { Start = startTimestamp };

            // set location
            location = Parsing.ParseRegionName(message["regioncode"], location);
            activity.Details = location;

            // set thumbnail            
            string largeImage;
            if (Parsing.ValidRegion(regionCode))
            {
                largeImage = regionCode.ToLower();
            }
            else
            {
                Dictionary<string, string> imageKeys = new Dictionary<string, string>
                {
                    { "Dreaming", "dreaming" },
                    { "Sleeping", "dreaming" },
                    { "Dead", "death" }
                };

                if (imageKeys.ContainsKey(mode))
                {
                    largeImage = imageKeys[mode];
                }
                else
                {
                    largeImage = "slugcat";
                }

            }
            activity.Assets = new Discord.ActivityAssets { LargeImage = largeImage };

            Console.WriteLine($"DiscordRelay.SetPresence : about to update activity : {activity.Details}");
            lock (discordLock)
            {
                discord.GetActivityManager().UpdateActivity(activity, updateHandler);
            }
        }

        private static void ExceptionLogger(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = e.ExceptionObject as Exception;
            if (exception is null) { return; }

            StreamWriter sw = File.AppendText(exceptionLogPath);
            sw.Write(exception.ToString() + "\n\n");
            sw.Close();
        }

        public static void Log(object message)
        {
            StreamWriter sw = File.AppendText(logPath);
            sw.Write(message.ToString() + "\n");
            sw.Close();
        }
    }
}
