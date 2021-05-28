using System.IO;
using BepInEx;

namespace RegionCast
{
    [BepInPlugin("casheww.region_cast_discord", "RegionCast", "0.6.0")]
    public class RegionCastPlugin : BaseUnityPlugin
    {
        System.Diagnostics.Process sideapp = null;
        public static RegionCastPlugin Instance { get; private set; }      // used for config machine
        public Transmitter Transmitter { get; private set; }


        public RegionCastPlugin()
        {
            sideapp = System.Diagnostics.Process.Start(sideappExePath);

            Transmitter = new Transmitter(this);
            Instance = this;

            Hooks.Apply();
        }

        public static OptionalUI.OptionInterface LoadOI() => new ConfigMenu();

        static string sideappExePath = Directory.GetCurrentDirectory() +
                Path.DirectorySeparatorChar + "RegionCast-DiscordGameSDK" +
                Path.DirectorySeparatorChar + "RegionCastApp.exe";

        
    }
}
