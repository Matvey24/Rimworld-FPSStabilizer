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

        public FPSStabilizer(ModContentPack content) : base(content)
        {
            HarmonyPatcher.init();
            settings = GetSettings<Settings>();
            Log.Message($"FPSStabilizer v1.1 loaded, patcher message: {HarmonyPatcher.message}, fps set to {1000 / HarmonyPatcher.target_frametime}");
        }
        public override string SettingsCategory()
        {
            return "FPSStabilizer";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            settings.DrawSettings(inRect);
        }
    }
}
