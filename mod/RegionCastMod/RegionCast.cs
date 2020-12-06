using System;
using System.IO;
using BepInEx;
using UnityEngine;

namespace RegionCast
{
    [BepInPlugin("casheww.region_cast_discord", "RegionCast", "1.0.0")]
    public class RegionCast : BaseUnityPlugin
    {
        System.Diagnostics.Process castRecApp = null;
        DateTime lastUpdate = DateTime.Now;
        Transmitter transmitter;

        public RegionCast()
        {
            transmitter = new Transmitter(this);

            On.RainWorld.Start += RainWorld_Start;
            On.RainWorldGame.ExitToMenu += RainWorldGame_ExitToMenu;
            On.Player.Update += Player_Update;
        }

        void RainWorld_Start(On.RainWorld.orig_Start orig, RainWorld self)
        {
            orig(self);

            string path = Directory.GetCurrentDirectory() + @"\RegionCast-DiscordGameSDK\RCApp.exe";
            castRecApp = System.Diagnostics.Process.Start(path);

            if (castRecApp is null)
            {
                Debug.LogError("RegionCast : RegionCastApp.exe was not found!\n" +
                    "\tMake sure the DiscordGameSDK was placed in the Rain World root directory " +
                    "i.e. next to the `Rain World.exe`. Always read the README ;)\n");
                Destroy(GetComponent<RegionCast>());
                return;
            }

            transmitter.SendUDP(Transmitter.GameMode.Menu);
        }

        void RainWorldGame_ExitToMenu(On.RainWorldGame.orig_ExitToMenu orig, RainWorldGame self)
        {
            orig(self);
            transmitter.SendUDP(Transmitter.GameMode.Menu);
        }

        void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
        {
            orig(self, eu);

            // 5 second cooldown - Player.Update is called every frame
            DateTime currentTime = DateTime.Now;
            if (currentTime.Subtract(lastUpdate) < TimeSpan.FromSeconds(5)) { return; }

            string currentLocationName;
            Transmitter.GameMode gameMode;

            if (!(self.room.world.region is null))
            {
                // player is in a region (the region doesn't not exist...)
                currentLocationName = self.room.world.region.name;

                gameMode = Transmitter.GetGameMode(self.slugcatStats.name);
            }
            else
            {
                // player is not in a region. This probably means they're in arena/sandbox mode
                currentLocationName = self.room.roomSettings.name;
                gameMode = Transmitter.GameMode.Arena;
            }

            lastUpdate = currentTime;
            transmitter.SendUDP(gameMode, currentLocationName);
        }
    }
}
