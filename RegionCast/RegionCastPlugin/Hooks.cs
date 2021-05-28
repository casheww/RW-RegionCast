using System;

namespace RegionCast
{
    class Hooks
    {
        public static string SlugName { get; private set; }
        public static int CycleNumber { get; private set; }
        public static int PlayerCount { get; private set; }
        static DateTime lastUpdate = DateTime.Now;


        public static void Apply()
        {
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

        static void MainMenuHook(On.Menu.MainMenu.orig_ctor orig, Menu.MainMenu self, ProcessManager manager, bool showRegionSpecificBkg)
        {
            orig(self, manager, showRegionSpecificBkg);
            RegionCastPlugin.Instance.Transmitter.SendUDP("Menu");
        }

        static void ExitToMenuHook(On.RainWorldGame.orig_ExitToMenu orig, RainWorldGame self)
        {
            orig(self);
            RegionCastPlugin.Instance.Transmitter.SendUDP("Menu");
        }

        static void SlugMenuSignalHook(On.Menu.SlugcatSelectMenu.orig_Singal orig, Menu.SlugcatSelectMenu self, Menu.MenuObject sender, string message)
        {
            orig(self, sender, message);
            if (message == "START")
            {
                SlugName = Utils.GetSlugName(self.slugcatPages[self.slugcatPageIndex].colorName);
            }
        }

        static void PlayerUpdateHook(On.Player.orig_Update orig, Player self, bool eu)
        {
            orig(self, eu);

            if (self.dead)
            {
                RegionCastPlugin.Instance.Transmitter.SendUDP("Dead");
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
                catch (ArgumentOutOfRangeException)
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
                Hooks.CycleNumber = cycleNumber;
            }
            else
            {
                // player is not in a region. This probably means they're in arena/sandbox mode
                currentLocationName = self.room.roomSettings.name;
                SlugName = "Arena";
            }

            PlayerCount = self.room.game.Players.Count;

            lastUpdate = currentTime;
            RegionCastPlugin.Instance.Transmitter.SendUDP(SlugName, currentLocationName, regionCode);
        }

        static void SleepHook(On.RainWorldGame.orig_Win orig, RainWorldGame self, bool malnourished)
        {
            orig(self, malnourished);
            RegionCastPlugin.Instance.Transmitter.SendUDP("Sleeping");
        }

        static void DreamHook(On.DreamsState.orig_InitiateEventDream orig, DreamsState self, DreamsState.DreamID dreamID)
        {
            orig(self, dreamID);
            RegionCastPlugin.Instance.Transmitter.SendUDP("Dreaming");
        }

        static void GameOverHook(On.RainWorldGame.orig_GameOver orig, RainWorldGame game, Creature.Grasp grasp)
        {
            orig(game, grasp);
            RegionCastPlugin.Instance.Transmitter.SendUDP("Dead");
        }
    }
}
