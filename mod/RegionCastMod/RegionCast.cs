using System;
using System.IO;
using BepInEx;
using UnityEngine;

namespace RegionCast
{
    [BepInPlugin("casheww.region_cast_discord", "RegionCast", "0.3.1")]
    public class RegionCast : BaseUnityPlugin
    {
        System.Diagnostics.Process castRecApp = null;
        DateTime lastUpdate = DateTime.Now;
        Transmitter transmitter;

        public string GameMode { get; set; }

        public RegionCast()
        {
            transmitter = new Transmitter(this);

            AddGameHooks();
        }

        void AddGameHooks()
        {
            On.RainWorld.Start += RainWorld_Start;
            On.Menu.MainMenu.ctor += MainMenu_ctor;
            On.RainWorldGame.ExitToMenu += RainWorldGame_ExitToMenu;
            On.Menu.SlugcatSelectMenu.Singal += SlugMenu_Signal;
            On.Player.Update += Player_Update;
        }

        void RainWorld_Start(On.RainWorld.orig_Start orig, RainWorld self)
        {
            orig(self);

            string path = Directory.GetCurrentDirectory() +
                Path.DirectorySeparatorChar + "RegionCast-DiscordGameSDK" +
                Path.DirectorySeparatorChar + "RCApp.exe";
            castRecApp = System.Diagnostics.Process.Start(path);

            if (castRecApp is null)
            {
                Debug.LogError("RegionCast : RegionCastApp.exe was not found!\n" +
                    "\tMake sure the DiscordGameSDK was placed in the Rain World root directory " +
                    "i.e. next to the `Rain World.exe`. Always read the README ;)\n");
                Destroy(GetComponent<RegionCast>());
                return;
            }
        }
        
        void MainMenu_ctor(On.Menu.MainMenu.orig_ctor orig, Menu.MainMenu self, ProcessManager manager, bool showRegionSpecificBkg)
        {
            orig(self, manager, showRegionSpecificBkg);
            transmitter.SendUDP("Menu");
        }

        void RainWorldGame_ExitToMenu(On.RainWorldGame.orig_ExitToMenu orig, RainWorldGame self)
        {
            orig(self);
            transmitter.SendUDP("Menu");
        }

        void SlugMenu_Signal(On.Menu.SlugcatSelectMenu.orig_Singal orig, Menu.SlugcatSelectMenu self, Menu.MenuObject sender, string message)
        {
            orig(self, sender, message);
            if (message == "START")
            {
                GameMode = Utils.GetSlugName(self.slugcatPages[self.slugcatPageIndex].colorName);
            }
        }

        void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
        {
            orig(self, eu);

            // 5 second cooldown - Player.Update is called every frame
            DateTime currentTime = DateTime.Now;
            if (currentTime.Subtract(lastUpdate) < TimeSpan.FromSeconds(5)) { return; }

            string currentLocationName;
            string regionCode = "";

            if (!(self.room.world.region is null))
            {
                regionCode = self.room.world.region.name;

                // player is in a region (the region doesn't not exist...)
                int regionNumber = self.room.abstractRoom.subRegion;
                if (regionNumber == 0)
                {
                    regionNumber = 1;
                }

                try
                {
                    currentLocationName = self.room.world.region.subRegions[regionNumber];
                }
                catch (IndexOutOfRangeException)
                {
                    currentLocationName = regionCode;
                }
            }
            else
            {
                // player is not in a region. This probably means they're in arena/sandbox mode
                currentLocationName = self.room.roomSettings.name;
                GameMode = "Arena";
            }

            int playerCount = self.room.game.Players.Count;

            lastUpdate = currentTime;
            transmitter.SendUDP(GameMode, currentLocationName, regionCode, playerCount);
        }
    }
}
