# Rain World - RegionCast
A BepInEx plugin for Rain World that adds Discord Rich Presence, now with support for region pack mods* !

![GitHub all releases](https://img.shields.io/github/downloads/casheww/RW-RegionCast/total?color=7185a8&style=for-the-badge)
![GitHub tag (latest by date)](https://img.shields.io/github/v/tag/casheww/RW-RegionCast?color=233f70&label=latest&style=for-the-badge)

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
4) - If using BepInEx with BOI, extract `RegionCast.dll` to `./Mods`
   - If using BepInEx without BOI, extract `RegionCast.dll` to `./BepInEx/plugins`
5) Extract the `RegionCast-DiscordGameSDK` folder to the Rain World root directory


## Troubleshooting

### Antivirus issues?
Your antivirus may not like a certain `RegionCastApp.exe` on the first run. This is to do with Rain World and the Discord Game SDK targetting different .NET versions, meaning the part of the mod that interacts with Discord needs to be run separately.

### .NET runtimes
If you do not have .NET 5 installed, you will need to install it. Instructions for checking your .NET versions can be found [here](https://docs.microsoft.com/en-us/dotnet/core/install/how-to-detect-installed-versions?pivots=os-windows#check-runtime-versions), and if you don't have a .NET Core 5.x version, you can find installation links for Windows, Mac, and Linux [a little further down on that page](https://docs.microsoft.com/en-us/dotnet/core/install/how-to-detect-installed-versions?pivots=os-windows#next-steps).


### Discord config
It should go without saying that Discord needs to be running on the same machine for this to work. You also need to enable the toggle switch in Discord settings > Game Activity.


Other issues? You can reach me in the modding channels of the [Rain World Discord](https://discord.gg/rainworld) (casheww) or by submitting a Github issue on this repository under the Issues tab.

---

\* For more details about region pack support, see [this  website](https://casheww.github.io/RW-RegionCast/).
Instructions for other content creators can be found [here](https://rain-world-modding.github.io/pages/utility-mods/RegionCast.html).

Many thanks to the modding folks in the Rain World Discord!
