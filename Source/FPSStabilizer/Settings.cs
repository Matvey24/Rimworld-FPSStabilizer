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

        float target_fps = 60;

        public bool input_method = false;
        bool prev_input_method = false;

        string text_input_buffer;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref target_fps, "target_fps", 60);
            Scribe_Values.Look(ref input_method, "input_method", false);
            SetFrametime();
        }
        public void DrawSettings(Rect inRect)
        {
            var list = new Listing_Standard();
            list.Begin(inRect);

            string label = $"Target FPS {target_fps:0.00}";

            if (input_method)
            {
                if (!prev_input_method)
                    text_input_buffer = "" + target_fps;
                list.TextFieldNumericLabeled(label, ref target_fps, ref text_input_buffer, 0, 10000);
            }
            else
                target_fps = RoundFPS(list.SliderLabeled(
                    label,
                    target_fps, 0, 120, 0.3f, ""));

            SetFrametime();

            prev_input_method = input_method;
           
            list.SubLabel($"Patcher Message: {HarmonyPatcher.message}", 1);
            list.CheckboxLabeled("Text input", ref input_method);
            list.SubLabel("This value determines, how long your cpu will do ticks until it switches to frames. " +
                "Usually you do not want Target FPS to be higher than your game maximum FPS, because it would make cpu just wait sometimes. " +
                "RimWorld default is 22. 60 should always work nice. Value applies automatically.", 1);
            list.End();
        }
        void SetFrametime()
        {
            HarmonyPatcher.target_frametime = 1000 / target_fps;
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
