using System;
using System.IO;
using BepInEx;
using UnityEngine;

namespace RegionCast
{
    [BepInPlugin("casheww.region_cast_discord", "RegionCast", "0.5.1")]
    public class RegionCast : BaseUnityPlugin
    {
        System.Diagnostics.Process castRecApp = null;
        DateTime lastUpdate = DateTime.Now;
        Transmitter transmitter;
        public static RegionCast instance { get; private set; }      // used for config machine

        public static string SlugName { get; private set; }
        public static int CycleNumber { get; private set; }
        public static int PlayerCount { get; private set; }

        public RegionCast()
        {
            transmitter = new Transmitter(this);
            instance = this;

            AddGameHooks();
        }

        public static OptionalUI.OptionInterface LoadOI()
        {
            return new ConfigMenu();
        }

        void AddGameHooks()
        {
            On.RainWorld.Start += RwStartHook;

            // menu
            On.Menu.MainMenu.ctor += MainMenuHook;
            On.RainWorldGame.ExitToMenu += ExitToMenuHook;

            // start game
            On.Menu.SlugcatSelectMenu.Singal += SlugMenuSignalHook;

            // player update
            On.Player.Update += PlayerUpdateHook;

            // scenes, dreams, death, etc.
            On.RainWorldGame.Win += SleepHook;
            On.DreamsState.InitiateEventDream += DreamHook;
            On.RainWorldGame.GameOver += GameOverHook;
        }

        void RwStartHook(On.RainWorld.orig_Start orig, RainWorld self)
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
        
        void MainMenuHook(On.Menu.MainMenu.orig_ctor orig, Menu.MainMenu self, ProcessManager manager, bool showRegionSpecificBkg)
        {
            orig(self, manager, showRegionSpecificBkg);
            transmitter.SendUDP("Menu");
        }

        void ExitToMenuHook(On.RainWorldGame.orig_ExitToMenu orig, RainWorldGame self)
        {
            orig(self);
            transmitter.SendUDP("Menu");
        }

        void SlugMenuSignalHook(On.Menu.SlugcatSelectMenu.orig_Singal orig, Menu.SlugcatSelectMenu self, Menu.MenuObject sender, string message)
        {
            orig(self, sender, message);
            if (message == "START")
            {
                SlugName = Utils.GetSlugName(self.slugcatPages[self.slugcatPageIndex].colorName);
            }
        }

        void PlayerUpdateHook(On.Player.orig_Update orig, Player self, bool eu)
        {
            orig(self, eu);

            if (self.dead)
            {
                transmitter.SendUDP("Dead");
                return;
            }

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

                StoryGameSession session = self.room.world.game.session as StoryGameSession;
                int cycleNumber = session.saveState.cycleNumber;
                if (SlugName == "Hunter")
                {
                    cycleNumber = 19 - cycleNumber;
                    if (session.saveState.redExtraCycles) cycleNumber += 5;
                }
                CycleNumber = cycleNumber;
            }
            else
            {
                // player is not in a region. This probably means they're in arena/sandbox mode
                currentLocationName = self.room.roomSettings.name;
                SlugName = "Arena";
            }

            PlayerCount = self.room.game.Players.Count;

            lastUpdate = currentTime;
            transmitter.SendUDP(SlugName, currentLocationName, regionCode);
        }

        void SleepHook(On.RainWorldGame.orig_Win orig, RainWorldGame self, bool malnourished)
        {
            orig(self, malnourished);
            transmitter.SendUDP("Sleeping");
        }

        void DreamHook(On.DreamsState.orig_InitiateEventDream orig, DreamsState self, DreamsState.DreamID dreamID)
        {
            orig(self, dreamID);
            transmitter.SendUDP("Dreaming");
        }

        void GameOverHook(On.RainWorldGame.orig_GameOver orig, RainWorldGame game, Creature.Grasp grasp)
        {
            orig(game, grasp);
            transmitter.SendUDP("Dead");
        }
    }
}
