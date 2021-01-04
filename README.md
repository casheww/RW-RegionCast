# Rain World - RegionCast
A BepInEx plugin for Rain World that adds Discord Rich Presence, now with support for region pack mods* !

![regioncast demo](https://github.com/casheww/RW-RegionCast/blob/main/assets/regioncastdemo.png)

[Skip to download instructions](https://github.com/casheww/RW-RegionCast#installation)

---

### Features
Displays the following game details on your Discord profile for other users to see while you play
- region thumbnail art
- region/arena name
- game mode / difficulty
- time spent in current game mode

---

### Installation
`.` in file paths represents the Rain World root directory.

1) You must have Rain World BepInEx installed and set up - this won't run on Partiality.   [BepInEx download](https://drive.google.com/file/d/1WcCCsS3ABBdO1aX-iJGeqeE07YE4Qv88/view) | [RW BepInEx tutorial](https://youtu.be/brDN_8uN6-U)
2) As of RegionCast 0.5, this plugin depends on Config Machine. Please see the release page for a note on this.
3) Download the first .zip from the first "Assets" dropdown from [here](https://github.com/casheww/RW-RegionCast/releases/)
4) Extract `RegionCast.dll` to `./BepInEx/plugins`
5) Extract the `RegionCast-DiscordGameSDK` folder to the Rain World root directory

Your antivirus may not like a certain `RCApp.exe` on the first run. This is to do with Rain World and the Discord Game SDK targetting different .NET versions, meaning the part of the mod that interacts with Discord needs to be run separately.

If you do not have .NET Core 3.1 installed, you will need to install it. Instructions for checking your .NET versions can be found [here](https://docs.microsoft.com/en-us/dotnet/core/install/how-to-detect-installed-versions?pivots=os-windows#check-runtime-versions), and if you don't have a .NET Core 3.1.x version, you can find installation links for Windows, Mac, and Linux [a little further down on that page](https://docs.microsoft.com/en-us/dotnet/core/install/how-to-detect-installed-versions?pivots=os-windows#next-steps).

It should go without saying that Discord needs to be running on the same machine for this to work. You also need to enable the toggle switch in Discord settings > Game Activity.

Any issues? You can reach me in the modding channels of the [Rain World Discord](https://discord.gg/rainworld) (casheww) or by submitting a Github issue on this repository under the Issues tab.

---

\* For more details about region pack support, see [this  website](https://casheww.github.io/RW-RegionCast/).
For custom slugcat info, see [the slugcat page on the wiki](https://github.com/casheww/RW-RegionCast/wiki/Custom-Slugcats).

For people looking to clone and build themselves, you'll need to download the Discord Game SDK from [here](https://discord.com/developers/docs/game-sdk/sdk-starter-guide). Add the files from the `csharp` dir of the SDK source to `app/RegionCastApp/`, modify `Core.cs` as shown [here](https://github.com/discord/gamesdk-and-dispatch/issues/102#issuecomment-702414654). You may be able to get away with just adding Core, Constants, and ActivityManager, but I haven't tested this. I've also removed one of the csproj files for path reasons, so you probably want to make your own project and copy in the files.

**Mod** : .NET Framework 3.5<br>
**Side-app** : .NET Core 3.1

Many thanks to the modding folks in the Rain World Discord!
