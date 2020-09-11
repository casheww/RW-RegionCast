using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using BepInEx;
using UnityEngine;


namespace RegionCast
{
    [BepInPlugin("casheww.region_cast_discord_rp", "RegionCast", "0.1")]
    public class RichPresTrigger : BaseUnityPlugin
    {
        public string lastLocationName;
        public System.Diagnostics.Process castAppProc = null;
        public DateTime lastUpdate = DateTime.Now;

        public RichPresTrigger()
        {
            On.RainWorld.Start += RainWorld_Start;
            On.RainWorldGame.ExitToMenu += RainWorldGame_ExitToMenu;
            On.RainWorldGame.ExitGame += RainWorldGame_ExitGame;

            On.Player.Update += Player_Update;
        }

        void OnApplicationQuit()
        {
            try
            {
                castAppProc.Kill();
            }
            catch (InvalidOperationException)
            {
                Debug.LogError("tried to kill RegionCastApp but it wasn't open");
            }
        }

        private void RainWorld_Start(On.RainWorld.orig_Start orig, RainWorld self)
        {
            orig.Invoke(self);
            string path = Directory.GetCurrentDirectory() + @"\DiscordGameSDK\RegionCastApp.exe";
            castAppProc = System.Diagnostics.Process.Start(path);
        }

        private void RainWorldGame_ExitToMenu(On.RainWorldGame.orig_ExitToMenu orig, RainWorldGame self)
        {
            orig.Invoke(self);
            SendRP_UDP("Menu");
        }
        private void RainWorldGame_ExitGame(On.RainWorldGame.orig_ExitGame orig, RainWorldGame self, bool asDeath, bool asQuit)
        {
            orig.Invoke(self, asDeath, asQuit);
            SendRP_UDP("Menu");
        }

        private void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
        {
            orig.Invoke(self, eu);

            var currentTime = DateTime.Now;
            if (currentTime.Subtract(lastUpdate) < TimeSpan.FromSeconds(5)) { return; }

            string currentLocationName = (self.room.world.region == null) ? self.room.roomSettings.name : self.room.world.region.name;
            lastUpdate = currentTime;            
            lastLocationName = currentLocationName;
            SendRP_UDP(currentLocationName);
        }

        public void SendRP_UDP(string location)
        {
            var s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            var ip = IPAddress.Parse("127.0.0.1");
            byte[] locationBytes = Encoding.ASCII.GetBytes(location);
            var endpoint = new IPEndPoint(ip, 49181);

            s.SendTo(locationBytes, endpoint);
            Debug.Log("=== UDP sent!");
        }
    }
}
