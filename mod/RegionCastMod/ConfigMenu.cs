using System;
using OptionalUI;
using UnityEngine;

namespace RegionCast
{
    class ConfigMenu : OptionInterface
    {
        public ConfigMenu() : base()
        {

        }

        public override void Initialize()
        {
            base.Initialize();

            Tabs = new OpTab[1];
            Tabs[0] = new OpTab("cast options");
            Tabs[0].AddItems(Elements.all);
        }

        public override void Update(float dt)
        {
            base.Update(dt);
        }

        public override void ConfigOnChange()
        {
            base.ConfigOnChange();
        }

        class Elements
        {
            public static UIelement[] all = new UIelement[]
            {
                labelID
            };

            static OpLabel labelID = new OpLabel(
                new Vector2(100f, 550f), new Vector2(400f, 40f),
                "RegionCast", FLabelAlignment.Center, true
                );
        }
    }
}
