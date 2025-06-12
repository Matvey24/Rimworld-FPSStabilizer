using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace FPSStabilizer
{
    class FPSStabilizer : Mod
    {
        Settings settings;
        HarmonyPatcher patcher;

        public FPSStabilizer(ModContentPack content) : base(content)
        {
            patcher = new HarmonyPatcher();
            settings = GetSettings<Settings>();
            patcher.patch(settings.target_fps);
        }
        public override string SettingsCategory()
        {
            return "FPSStabilizer";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            if(settings.DrawSettings(inRect))
            {
                patcher.patch(settings.target_fps);
            }
        }
    }
}
