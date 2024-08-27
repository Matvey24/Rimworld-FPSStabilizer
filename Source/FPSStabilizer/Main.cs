using Verse;
namespace FPSStabilizer
{
    using HarmonyLib;
    using System.Reflection;
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using System.Linq;
    using UnityEngine;

    [StaticConstructorOnStartup]
    public static class Main
    {
        static Main(){
            Harmony harm = new Harmony("matvey24.FPSStabilizer");
            MethodBase original = AccessTools.Method(typeof(TickManager), nameof(TickManager.TickManagerUpdate));
            HarmonyMethod transpiler = new HarmonyMethod(typeof(Main), nameof(Transpiler));
            try
            {
                harm.Patch(original, null, null, transpiler, null);
            }
            catch (Exception e)
            {
                Log.Error($"Error during patching {original} with: transpliter {transpiler?.method}\n{e}");
            }
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeInstruction to_patch = null;

            foreach (var c in instructions)
            {
                if (c.opcode != OpCodes.Ldc_R4 || (float)c.operand < 10)
                    continue;

                if (to_patch != null)
                {
                    Log.Error("FPSStabilier not patched, two similar instructions was found");
                    return instructions;
                }
                
                to_patch = c;
            }
            if (to_patch == null) {
                Log.Error("FPSStabilier not patched, no any instruction was found");
                return instructions;
            }

            int rate = Application.targetFrameRate;
            float time = 1000f / rate;

            if (rate == 0) 
                // in my mind 0 can happen, if app is stopped (waiting mode)
                time = 16;

            if(rate < 0)
                // theoretically, -1 can be, if target framerate is unlimited, but not sure
                time = 1;

            if (rate > 1000)
                // prevent (int)frametime to be 0
                time = 1;

            to_patch.operand = time;
            Log.Message("FPSStabilier patched to " + 1000f / time + " FPS");
            return instructions;
        }
    }
}
