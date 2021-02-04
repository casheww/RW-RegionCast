using System;
using OptionalUI;
using UnityEngine;

namespace RegionCast
{
    class ConfigMenu : OptionInterface
    {
        public ConfigMenu() : base(plugin: RegionCast.Instance)
        {
            
        }

        public override void Initialize()
        {
            base.Initialize();
            Tabs = new OpTab[1];
            Tabs[0] = new OpTab("RichPresence");

            string modName = RegionCast.Instance.Info.Metadata.Name;
            string modVrsn = RegionCast.Instance.Info.Metadata.Version.ToString();
            string modAuth = RegionCast.Instance.Info.Metadata.GUID.Split('.')[0];

            Tabs[0].AddItems(new UIelement[]
            {
                new OpLabel(new Vector2(100f, 550f), new Vector2(400f, 40f), modName, bigText: true),
                new OpLabel(new Vector2(150f, 500f), new Vector2(150f, 30f), $"v{modVrsn}"),
                new OpLabel(new Vector2(300f, 500f), new Vector2(150f, 30f), $"by {modAuth}")
            });

            OpRadioButtonGroup buttonGroup = new OpRadioButtonGroup("toAppend", 0);
            Tabs[0].AddItems(new UIelement[] { buttonGroup });
            buttonGroup.SetButtons(new OpRadioButton[]
            {
                new OpRadioButton( 40f, 400f) { description = "No data is appended to your game mode" },
                new OpRadioButton(240f, 400f) { description = "Cycle count is appended to your game mode" },
                new OpRadioButton(440f, 400f) { description = "Player count (if greater than 1) is appended to your game mode" }
            });

            Tabs[0].AddItems(new UIelement[]
            {
                new OpLabel(new Vector2(100f, 430f), new Vector2(400f, 40f), "Data to append to game mode:"),
                new OpLabel(new Vector2( 80f, 400f), new Vector2(150f, 30f), "None", FLabelAlignment.Left),
                new OpLabel(new Vector2(280f, 400f), new Vector2(150f, 30f), "Cycle count", FLabelAlignment.Left),
                new OpLabel(new Vector2(480f, 400f), new Vector2(150f, 30f), "Player count", FLabelAlignment.Left)
            });
        }

        public override void Update(float dt)
        {
            base.Update(dt);
        }

        public override void ConfigOnChange()
        {
            base.ConfigOnChange();
            AppendSetting = (Append)int.Parse(config["toAppend"]);
        }

        public static Append AppendSetting { get; private set; }
        public enum Append
        {
            None,
            Cycles,
            Players
        }
    }
}
