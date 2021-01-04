using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace RegionCast
{
    class Transmitter
    {
        static readonly Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint endpoint;

        public Transmitter(RegionCast mod)
        {
            string configPath = Directory.GetCurrentDirectory() +
                Path.DirectorySeparatorChar + "RegionCast-DiscordGameSDK" +
                Path.DirectorySeparatorChar + "config.txt";
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

        public void SendUDP(string gameMode, string location = "", string regionCode = "")
        {
            /* Sends a UDP message to localhost:49181 where the RegionCastApp should be listening.
             * gameMode : the current game mode 
             * location : code of current region / name of current arena */

            gameMode += Utils.GameModeAppend(gameMode);

            string data = $"rwRegionCastData\n" +
                $"gamemode:{gameMode}\n" +
                $"location:{location}\n" +
                $"regioncode:{regionCode}";
            byte[] message = Encoding.UTF8.GetBytes(data);
            
            socket.SendTo(message, endpoint);
            Debug.Log($"RegionCast : UDP send to {endpoint.Address}:{endpoint.Port} for RCApp.exe");
        }
    }
}
