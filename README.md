# Rain World - RegionCast
A BepInEx plugin for Rain World that adds Discord Rich Presence, now with support for region pack mods* !

![regioncast demo](https://github.com/casheww/RW-RegionCast/blob/main/regioncastdemo.png)

PLEASE SCROLL DOWN FOR INSTALLATION INSTRUCTIONS

---

### Features
Displays the following game details on your Discord profile for other users to see while you play
- region thumbnail art
- region/arena name
- game mode / difficulty
- time spent in current game mode


### Installation
`.` in file paths represents the Rain World root directory.

1) You must have Rain World BepInEx installed and set up - this won't run on Partiality.   [BepInEx download](https://drive.google.com/file/d/1WcCCsS3ABBdO1aX-iJGeqeE07YE4Qv88/view) | [Rain World Discord](https://discord.gg/rainworld)
2) Download the first .zip from the first "Assets" dropdown from [here](https://github.com/casheww/RW-RegionCast/releases/)
3) Extract `RegionCast.dll` to `./BepInEx/plugins`
4) Extract the `RegionCast-DiscordGameSDK` folder to the Rain World root directory

Your antivirus may not like a certain `RCApp.exe` on the first run. This is to do with Rain World and the Discord Game SDK targetting different .NET versions, meaning the part of the mod that interacts with Discord needs to be run separately.

It should go without saying that Discord needs to be running on the same machine for this to work.

Any issues? You can reach me on the Rain World Discord or by submitting a Github issue on this repository under the Issues tab.

### * Region Pack Mods
Custom region thumbnails can only be supported if the region creator(s) submit a request to have their region thumbnail added. For more details about this, see [this](https://casheww.github.io/RW-RegionCast/) website.

---

Special thanks to [Fyre](https://github.com/FyreByrns), bee, [thalber](https://github.com/thalber), [Garrakx](https://github.com/Garrakx), Slime_Cubed, [LeeMoriya](https://github.com/LeeMoriya), and DryCryCrystal.
