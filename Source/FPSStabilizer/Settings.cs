using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace FPSStabilizer
{
    class Settings : ModSettings
    {
        public float target_fps = 60;
        public bool input_method = false;
        bool prev_input_method = false;

        string text_input_buffer;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref target_fps, "target_fps", 60);
            Scribe_Values.Look(ref input_method, "input_method", false);
        }
        public bool DrawSettings(Rect inRect)
        {
            var list = new Listing_Standard();
            list.Begin(inRect);

            string label = string.Format("Target FPS {0:F1}", target_fps);

            if (input_method)
            {
                if (!prev_input_method)
                    text_input_buffer = "" + target_fps;
                list.TextFieldNumericLabeled(label, ref target_fps, ref text_input_buffer, 0, 10000);
            }
            else
                target_fps = RoundFPS(list.SliderLabeled(
                    label,
                    target_fps, 0, 120, 0.3f,
                    "Rimworld default is 22"));

            prev_input_method = input_method;

            bool patchable = HarmonyPatcher.patchable;
            bool apply;
            if (patchable)
                apply = list.ButtonTextLabeled(HarmonyPatcher.message, "Apply");
            else
            {
                list.SubLabel(HarmonyPatcher.message, 1);
                apply = false;
            }
            list.CheckboxLabeled("Text input", ref input_method);

            list.End();
            return apply;
        }
        float RoundFPS(float fps)
        {
            if(fps > 5)
            {
                fps = Mathf.Round(fps);
            }
            else
            {
                fps = Mathf.Round(fps * 10) / 10;
            }
            return fps;
        }
    }
}
