using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace RegionCastMod
{
    class Transmitter
    {
        static readonly Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint endpoint;

        public Transmitter(RegionCast mod)
        {
            string configPath = Directory.GetCurrentDirectory() + @"\RegionCast-DiscordGameSDK\config.txt";
            string[] config = File.ReadAllLines(configPath);

            AttemptToMakeEndpoint(config, mod);
        }

        void AttemptToMakeEndpoint(string[] config, RegionCast mod)
        {
            int port;
            try
            {
                port = Convert.ToInt32(config[0]);
            }
            catch (FormatException)
            {
                Debug.LogError("RegionCast : first line of RegionCast-DiscordGameSDK\\config.txt " +
                    "(port number) could not be formatted to an int.");
                UnityEngine.Object.Destroy(mod.GetComponent<RegionCast>());
                return;
            }

            endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
        }

        public enum GameMode
        {
            Menu,
            Monk,
            Survivor,
            Hunter,
            Arena
            // todo: co-op compat?
        }

        public static GameMode GetGameMode(SlugcatStats.Name difficulty)
        {
            switch (difficulty)
            {
                case SlugcatStats.Name.Yellow:
                    return GameMode.Monk;

                default:
                case SlugcatStats.Name.White:
                    return GameMode.Survivor;

                case SlugcatStats.Name.Red:
                    return GameMode.Hunter;
            }
        }

        public void SendUDP(GameMode gameMode, string location = "", int playerCount = 0)
        {
            /* Sends a UDP message to localhost:49181 where the RegionCastApp should be listening.
             * gameMode : the current game mode 
             * location : code of current region / name of current arena
             * playerCount : may be used in future for extra compat features with Jolly Coop / Monkland */

            string data = $"rwRegionCastData\n" +
                $"gamemode:{gameMode}\n" +
                $"location:{location}\n" +
                $"playercount:{playerCount}";
            byte[] message = Encoding.UTF8.GetBytes(data);
            
            socket.SendTo(message, endpoint);
            Debug.Log($"RegionCast : UDP send to {endpoint.Address}:{endpoint.Port} for RegionCastApp");
        }
    }
}
